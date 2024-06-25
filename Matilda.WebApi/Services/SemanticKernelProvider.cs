using Matilda.WebApi.Options;
using Microsoft.Extensions.Options;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;

namespace Matilda.WebApi.Services;

/// <summary>
/// Extension methods for registering Semantic Kernel related services.
/// </summary>
public class SemanticKernelProvider(IServiceProvider serviceProvider, IConfiguration configuration, IHttpClientFactory httpClientFactory)
{
    private readonly IKernelBuilder _kernelBuilder = InitializeCompletionKernel(serviceProvider, configuration, httpClientFactory);

    /// <summary>
    /// Produce semantic-kernel with only completion services for chat.
    /// </summary>
    public Kernel GetCompletionKernel() => _kernelBuilder.Build();
    
    private static IKernelBuilder InitializeCompletionKernel(IServiceProvider serviceProvider, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        var builder = Kernel.CreateBuilder();
        builder.Services.AddLogging();

        var options = serviceProvider.GetRequiredService<IOptions<SemanticKernelOptions>>().Value;
        switch (options.TextGeneratorType)
        {
            case {} t when t.Equals("AzureOpenAI", StringComparison.OrdinalIgnoreCase):
                // ReSharper disable once InconsistentNaming
                var azureOpenAIOptions = options.GetConfig<AzureOpenAIConfig>(configuration, "AzureOpenAI");
                builder.AddAzureOpenAIChatCompletion(azureOpenAIOptions.Deployment, azureOpenAIOptions.Endpoint, azureOpenAIOptions.APIKey, httpClient: httpClientFactory.CreateClient());
                break;
            
            case {} t when t.Equals("OpenAI", StringComparison.OrdinalIgnoreCase):
                // ReSharper disable once InconsistentNaming
                var openAIOptions = options.GetConfig<OpenAIConfig>(configuration, "OpenAI");
                builder.AddOpenAIChatCompletion(openAIOptions.TextModel, openAIOptions.APIKey, httpClient: httpClientFactory.CreateClient());
                break;
            
            default:
                throw new ArgumentException($"Invalid {nameof(options.TextGeneratorType)} '{options.TextGeneratorType}' value in 'SemanticKernel' settings.");
        }

        return builder;
    }
}
