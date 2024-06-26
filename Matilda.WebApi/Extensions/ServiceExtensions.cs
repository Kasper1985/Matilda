using Matilda.WebApi.Models;
using Matilda.WebApi.Options;
using Matilda.WebApi.Storage;
using Microsoft.Extensions.Options;

namespace Matilda.WebApi.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        // Add options with initial prompts
        AddOptions<PromptsOptions>(PromptsOptions.SectionName);
        
        // Add options with semantic kernel configuration
        AddOptions<SemanticKernelOptions>(SemanticKernelOptions.SectionName);
        
        // Add options with chat storage configuration
        AddOptions<ChatStoreOptions>(ChatStoreOptions.SectionName);
        
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

    public static IServiceCollection AddChatStore(this IServiceCollection services)
    {
        var chatStoreOptions = services.BuildServiceProvider().GetRequiredService<IOptions<ChatStoreOptions>>().Value;

        switch (chatStoreOptions.Type)
        {
            case {} t when t.Equals("InMemory", StringComparison.OrdinalIgnoreCase):
                services.AddSingleton(new Repository<ChatMessage>(new InMemoryContext<ChatMessage>()));
                break;
            
            case {} t when t.Equals("Filesystem", StringComparison.OrdinalIgnoreCase):
                var fullPath = Path.GetFullPath(chatStoreOptions.Filesystem!.FilePath);
                var directory = Path.GetDirectoryName(fullPath) ?? string.Empty;
                var fileInfo = new FileInfo(Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(fullPath)}.messages{Path.GetExtension(fullPath)}"));
                services.AddSingleton(new Repository<ChatMessage>(new FileSystemContext<ChatMessage>(fileInfo)));
                break;
            
            case {} t when t.Equals("MongoDB", StringComparison.OrdinalIgnoreCase):
                var mongoDbContext = new MongoDbContext<ChatMessage>(chatStoreOptions.MongoDb!.ConnectionString, chatStoreOptions.MongoDb!.DatabaseName);
                services.AddSingleton(new Repository<ChatMessage>(mongoDbContext));
                break;
            
            default:
                throw new ArgumentOutOfRangeException($"Invalid chat store type: {chatStoreOptions.Type}");
        }

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
