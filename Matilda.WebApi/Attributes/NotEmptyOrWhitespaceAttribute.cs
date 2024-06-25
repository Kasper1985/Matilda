using System.ComponentModel.DataAnnotations;

namespace Matilda.WebApi.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class NotEmptyOrWhitespaceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context) => value switch
    {
        null => ValidationResult.Success,
        string s when !string.IsNullOrWhiteSpace(s) => ValidationResult.Success,
        string => new ValidationResult($"'{context.MemberName}' cannot be empty or whitespace."),
        _ => new ValidationResult($"'{context.MemberName}' must be a string.")
    };
}
