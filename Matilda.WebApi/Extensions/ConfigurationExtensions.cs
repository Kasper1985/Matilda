namespace Matilda.WebApi.Extensions;

public static class ConfigurationExtensions
{
    public static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        
        // Load configurations from appsettings.json and its environment-specific counterpart.
        configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        configuration.AddJsonFile("appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
        
        // Add user secrets for development environments.
        if (builder.Environment.IsDevelopment())
            configuration.AddUserSecrets<Program>();
        
        // Override existing configurations with local environment configurations.
        configuration.AddEnvironmentVariables();
        
        return builder;
    }    
}
