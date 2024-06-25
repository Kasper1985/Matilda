using System.ComponentModel.DataAnnotations;
using Matilda.WebApi.Attributes;
using Microsoft.KernelMemory;

namespace Matilda.WebApi.Options;

public class SemanticKernelOptions
{
    public const string SectionName = "SemanticKernel";
    
    [Required, NotEmptyOrWhitespace] public string TextGeneratorType { get; init; } = string.Empty;
    
    public T GetConfig<T>(IConfiguration configuration, string sectionName, string root = SectionName)
    {
        return configuration
            .GetSection(root)
            .GetSection(sectionName)
            .Get<T>() ?? throw new ConfigurationException($"The {sectionName} configuration could not be found.");
    }
}
