using Azure.AI.OpenAI;
using Matilda.WebApi.Models;
using Matilda.WebApi.Options;
using Matilda.WebApi.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Matilda.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController(ILogger<ChatController> logger, IOptions<PromptsOptions> promptOptions) : ControllerBase
{
    private readonly OpenAIPromptExecutionSettings _executionSettings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };
    
    [HttpPost]
    public async Task<IActionResult> Chat(
        [FromServices] Kernel kernel,
        [FromServices] Repository<ChatMessage> chatMessageRepository,
        [FromBody] Ask ask)
    {
        logger.LogDebug("Chat message received.");
        
        try
        {
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            var chatHistory = await GetChatHistory(chatMessageRepository);
            await AddMessageToChatHistory(chatHistory, chatMessageRepository, new ChatMessage
            {
                Role = AuthorRole.User,
                Content = ask.Content,
                TimeStamp = DateTimeOffset.Now
            });
            var result  = await chatCompletionService.GetChatMessageContentAsync(chatHistory, _executionSettings, kernel);
            logger.LogDebug("Chat message processed.");
            
            var completionUsage = result.Metadata!["Usage"] as CompletionsUsage;
            var message = new ChatMessage
            {
                Role = result.Role,
                Content = result.Content ?? string.Empty,
                TimeStamp = DateTimeOffset.Now,
                TokenUsage = new Dictionary<string, int>
                {
                    { nameof(completionUsage.PromptTokens), completionUsage?.PromptTokens ?? 0 },
                    { nameof(completionUsage.CompletionTokens), completionUsage?.CompletionTokens ?? 0 },
                    { nameof(completionUsage.TotalTokens), completionUsage?.TotalTokens ?? 0 }
                }
            };
            var response = new Answer
            {
                Content = message.Content,
                TokenUsage = message.TokenUsage[nameof(completionUsage.TotalTokens)]
            };
            await AddMessageToChatHistory(chatHistory, chatMessageRepository, message);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Chat message processing failed.");
            throw;
        }
    }

    private async Task<ChatHistory> GetChatHistory(Repository<ChatMessage> repository)
    {
        var chatHistory = new ChatHistory();
        foreach (var message in await repository.Find(_ => true))
            chatHistory.AddMessage(message.Role, message.Content);

        if (chatHistory.Count < 1)
        {
            await AddMessageToChatHistory(chatHistory, repository, new ChatMessage { Role = AuthorRole.System, Content = promptOptions.Value.SystemDescription, TimeStamp = DateTimeOffset.Now });
            await AddMessageToChatHistory(chatHistory, repository, new ChatMessage { Role = AuthorRole.Assistant, Content = promptOptions.Value.InitialMessage, TimeStamp = DateTimeOffset.Now });
        }

        return chatHistory;
    }
    
    private static async Task AddMessageToChatHistory(ChatHistory chatHistory, Repository<ChatMessage> repository, ChatMessage message)
    {
        chatHistory.AddMessage(message.Role, message.Content);
        await repository.Create(message);
    }
}
