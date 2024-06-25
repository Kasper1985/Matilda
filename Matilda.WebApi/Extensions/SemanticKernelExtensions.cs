using Matilda.WebApi.Plugins;
using Matilda.WebApi.Services;
using Microsoft.SemanticKernel;

namespace Matilda.WebApi.Extensions;

/// <summary>
/// Extension methods for registering Semantic Kernel related services.
/// </summary>
public static class SemanticKernelExtensions
{
    /// <summary>
    /// Delegate to register functions with a kernel.
    /// </summary>
    private delegate Task RegisterKernelFunctions(IServiceProvider serviceProvider, Kernel kernel);
    
    public static WebApplicationBuilder AddSemanticKernelServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(sp => new SemanticKernelProvider(sp, builder.Configuration, sp.GetRequiredService<IHttpClientFactory>()));
        
        // Register kernel functions
        builder.Services.AddScoped<RegisterKernelFunctions>(_ => RegisterPlugins);
        
        // Register semantic kernel
        builder.Services.AddScoped<Kernel>(sp =>
        {
            var semanticKernelProvider = sp.GetRequiredService<SemanticKernelProvider>();
            var kernel = semanticKernelProvider.GetCompletionKernel();

            sp.GetRequiredService<RegisterKernelFunctions>()(sp, kernel);

            return kernel;
        });

        return builder;
    }
    
    private static Task RegisterPlugins(IServiceProvider serviceProvider, Kernel kernel)
    {
        // Register plugins
        kernel.ImportPluginFromType<LightPlugin>(nameof(LightPlugin));
        
        return Task.CompletedTask;
    }
}
