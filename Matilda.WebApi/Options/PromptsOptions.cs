using System.ComponentModel.DataAnnotations;
using Matilda.WebApi.Attributes;

namespace Matilda.WebApi.Options;

public class PromptsOptions
{
    public const string SectionName = "Prompts";

    [Required, NotEmptyOrWhitespace] public string SystemDescription { get; init; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string InitialMessage { get; init; } = string.Empty;
    [Required, NotEmptyOrWhitespace] public string MessageIntent { get; init; } = string.Empty;
}
