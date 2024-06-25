using System.Text.Json;
using Matilda.WebApi.Extensions;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

var builder = WebApplication.CreateBuilder(args);

// Load configurations
builder.AddConfigurations()
    .WebHost.UseUrls(); // Disables endpoint override warning message when using IConfiguration for Kestrel endpoint.

// Add configuration options and required services.
builder.Services
    .AddSingleton<ILogger>(sp => sp.GetRequiredService<ILogger<Program>>())
    .AddOptions(builder.Configuration);

// Configure and add semantic services.
builder.AddSemanticKernelServices();

builder.Services
    .AddHttpClient() // Add named HTTP clients for IHttpClientFactory
    .AddEndpointsApiExplorer() // Configuring API Explorer
    .AddSwaggerGen() // Configuring Swagger
    .AddCorsPolicy(builder.Configuration) // Configuring CORS
    .AddControllers() // Register existing controllers
    .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; }); // Adds JSON serialization options
builder.Services.AddHealthChecks(); // Add health checks

// Configure middleware and endpoints
var app = builder.Build();
app.UseCors();
app.MapControllers();
    //.RequireAuthorization(); // Uncomment this line to require authorization for all controllers
app.MapHealthChecks("/healthz");

// Enable Swagger for development environments.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();
    
    // Introduce redirection from root URL to Swagger UI URL
    app.MapWhen(context => context.Request.Path == "/", appBuilder => appBuilder.Run(async context => await Task.Run(() => context.Response.Redirect("/swagger"))));
}

// Using base path
app.UsePathBase(builder.Configuration.GetValue<string>("Api_BasePath"));

// Start the service.
var runTask = app.RunAsync();

// Log the health probe URL for users to validate the series is running.
try
{
    var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()?.Addresses?.FirstOrDefault();
    app.Services.GetRequiredService<ILogger>().LogInformation("Health probe: {address}/healthz", address);
}
catch (ObjectDisposedException)
{
    // The startup is likely failed witch disposes 'app.Services' - don't attempt to display the health probe URL.
}

// Wait for the service to complete.
await runTask;
