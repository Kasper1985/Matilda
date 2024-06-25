using Azure.AI.OpenAI;
using Matilda.WebApi.Models;
using Matilda.WebApi.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Matilda.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly IOptions<PromptsOptions> _promptOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Kernel _kernel;
    private readonly ILogger<ChatController> _logger;
    private readonly IChatCompletionService _chatCompletionService;
    
    private readonly OpenAIPromptExecutionSettings _executionSettings = new OpenAIPromptExecutionSettings { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

    public ChatController(IOptions<PromptsOptions> promptOptions, IHttpClientFactory httpClientFactory, Kernel kernel, ILogger<ChatController> logger)
    {
        _promptOptions = promptOptions ?? throw new ArgumentNullException(nameof(promptOptions));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }
    
    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] Ask ask)
    {
        _logger.LogDebug("Chat message received.");
        
        var chatHistory = new ChatHistory(new []
        {
            new ChatMessageContent(AuthorRole.System, _promptOptions.Value.InitialMessage),
            new ChatMessageContent(AuthorRole.User, ask.Input)
        });

        try
        {
            var result  = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, _executionSettings, _kernel);
            _logger.LogDebug("Chat message processed.");
            var response = new Answer
            {
                Content = result.Content ?? string.Empty,
                TokenUsage = ((CompletionsUsage)result.Metadata!["Usage"]!).TotalTokens
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chat message processing failed.");
            throw;
        }
    }
}
