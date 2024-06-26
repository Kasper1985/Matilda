using System.ComponentModel.DataAnnotations;
using Matilda.WebApi.Attributes;

namespace Matilda.WebApi.Models;

public class Ask
{
    [Required, NotEmptyOrWhitespace] public string Content { get; init; } = string.Empty;
}
