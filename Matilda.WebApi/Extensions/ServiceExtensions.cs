using Matilda.WebApi.Options;

namespace Matilda.WebApi.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        // Add options with initial prompts
        AddOptions<PromptsOptions>(PromptsOptions.SectionName);
        
        // Add options with semantic kernel configuration
        AddOptions<SemanticKernelOptions>(SemanticKernelOptions.SectionName);
        
        return services;
        
        // Local function for simplifying the addition of options for a given section
        // ReSharper disable once LocalFunctionHidesMethod
        void AddOptions<TOptions>(string sectionName) where TOptions : class
        {
            services.AddOptions<TOptions>(configuration.GetSection(sectionName));
        }
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
        if (allowedOrigins.Length > 0)
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                        .WithMethods("POST", "GET", "PUT", "DELETE", "PATCH")
                        .AllowAnyHeader();
                });
            });

        return services;
    }

    private static void AddOptions<TOptions>(this IServiceCollection services, IConfigurationSection section) where TOptions : class
    {
        services.AddOptions<TOptions>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart()
            .PostConfigure(TrimStringProperties);
    }
    
    /// <summary>
    /// Trim all string properties of the given options object recursively.
    /// </summary>
    private static void TrimStringProperties<T>(T options) where T : class
    {
        var targets = new Queue<object>();
        targets.Enqueue(options);
        
        while (targets.Count > 0)
        {
            var target = targets.Dequeue();
            var targetType = target.GetType();
            foreach (var property in targetType.GetProperties())
            {
                // Skip enumerations
                if (property.PropertyType.IsEnum) continue;
                // Skip index properties
                if (property.GetIndexParameters().Length > 0) continue;
                
                // Property is a build-in type, readable and writable
                if (property.PropertyType.Namespace == "System" && property is { CanRead: true, CanWrite: true })
                {
                    // Property is a non-null string
                    if (property.PropertyType == typeof(string) && property.GetValue(target) is not null)
                        property.SetValue(target, property.GetValue(target)!.ToString()!.Trim());
                }
                else
                {
                    // Property is a non-built-in and non-enum type - queue it for further processing
                    if (property.GetValue(target) is not null)
                        targets.Enqueue(property.GetValue(target)!);
                }
            }
        }
    }
}
