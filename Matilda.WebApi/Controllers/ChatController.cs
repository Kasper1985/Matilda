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
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Chat>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Chats([FromServices] Repository<Chat> chatRepository)
    {
        logger.LogDebug("All chats requested.");

        var chats = await chatRepository.Find(_ => true);
        
        return Ok(chats);
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Chat))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Chat([FromServices] Repository<Chat> chatRepository, [FromRoute] Guid id)
    {
        logger.LogDebug("Chat history requested.");
        
        var chat = await chatRepository.FindById(id.ToString());
        
        return chat is not null ? Ok(chat) : NotFound();
    }

    [HttpPost("{title}")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Chat))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> NewChat([FromServices] Repository<Chat> chatRepository, [FromRoute]string title)
    {
        logger.LogDebug("New chat created.");
        
        var chat = await chatRepository.Create(new Chat { Title = title, TimeStamp = DateTimeOffset.Now });

        return CreatedAtAction(nameof(Chat), new { id = Guid.Parse(chat.Id) }, chat);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteChat([FromServices] Repository<Chat> chatRepository, [FromRoute] Guid id)
    {
        logger.LogDebug("Chat history deleted.");
        var chat = await chatRepository.FindById(id.ToString());
        if (chat is null)
            return NotFound();
        
        var result = await chatRepository.Delete(new Chat { Id = id.ToString() });

        return result ? NoContent() : NotFound();
    }


    [HttpGet("{id:guid}/messages")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ChatMessage>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetChatMessages(
        [FromServices] Repository<Chat> chatRepository,
        [FromServices] Repository<ChatMessage> chatMessageRepository,
        [FromRoute] Guid id)
    {
        var chat = await chatRepository.FindById(id.ToString());
        if (chat is null)
            return NotFound();
        
        var messages = await chatMessageRepository.Find(m => m.ChatId == id.ToString());
        
        return Ok(messages);
    }
    
    [HttpPost("{id:guid}/ask")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChatMessage))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendMessage(
        [FromServices] Kernel kernel,
        [FromServices] Repository<Chat> chatRepository,
        [FromServices] Repository<ChatMessage> chatMessageRepository,
        [FromRoute] Guid id,
        [FromBody] Ask ask)
    {
        logger.LogDebug("Chat message received.");
        
        var chat = await chatRepository.FindById(id.ToString());
        if (chat is null)
            return NotFound();
        
        try
        {
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            var chatHistory = await GetChatHistory(id.ToString(), chatMessageRepository);
            var requestMsg = await AddMessageToChatHistory(chatHistory, chatMessageRepository, new ChatMessage
            {
                ChatId = id.ToString(),
                Role = AuthorRole.User,
                Content = ask.Content,
                TimeStamp = DateTimeOffset.Now
            });
            var result  = await chatCompletionService.GetChatMessageContentAsync(chatHistory, _executionSettings, kernel);
            logger.LogDebug("Chat message processed.");
            
            var completionUsage = result.Metadata!["Usage"] as CompletionsUsage;
            var responseMsg = await AddMessageToChatHistory(chatHistory, chatMessageRepository, new ChatMessage
            {
                ChatId = id.ToString(),
                Role = result.Role,
                Content = result.Content ?? string.Empty,
                TimeStamp = DateTimeOffset.Now,
                TokenUsage = new Dictionary<string, int>
                {
                    { nameof(completionUsage.PromptTokens), completionUsage?.PromptTokens ?? 0 },
                    { nameof(completionUsage.CompletionTokens), completionUsage?.CompletionTokens ?? 0 },
                    { nameof(completionUsage.TotalTokens), completionUsage?.TotalTokens ?? 0 }
                }
            });
            
            return Ok(responseMsg);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Chat message processing failed.");
            throw;
        }
    }

    private async Task<ChatHistory> GetChatHistory(string id, Repository<ChatMessage> repository)
    {
        var chatHistory = new ChatHistory();
        foreach (var message in await repository.Find(m => m.ChatId == id))
            chatHistory.AddMessage(message.Role, message.Content);

        if (chatHistory.Count < 1)
        {
            await AddMessageToChatHistory(chatHistory, repository, new ChatMessage
            {
                ChatId = id,
                Role = AuthorRole.System,
                Content = promptOptions.Value.SystemDescription,
                TimeStamp = DateTimeOffset.Now
            });
            await AddMessageToChatHistory(chatHistory, repository, new ChatMessage
            {
                ChatId = id,
                Role = AuthorRole.Assistant,
                Content = promptOptions.Value.InitialMessage,
                TimeStamp = DateTimeOffset.Now
            });
        }

        return chatHistory;
    }
    
    private static async Task<ChatMessage> AddMessageToChatHistory(ChatHistory chatHistory, Repository<ChatMessage> repository, ChatMessage message)
    {
        chatHistory.AddMessage(message.Role, message.Content);
        return await repository.Create(message);
    }
}
