using System.ComponentModel.DataAnnotations;

namespace Matilda.WebApi.Attributes;

/// <summary>
/// If the other property is set to the expected value, then this property is required.
/// </summary>
/// <param name="otherPropertyName">Name of the other property.</param>
/// <param name="otherPropertyValue">Value of the other property when this property is required.</param>
/// <param name="notEmptyOrWhitespace">True to make sure that the value is not empty or whitespace when required.</param>
[AttributeUsage(AttributeTargets.Property)]
public class RequiredOnPropertyValueAttribute(string otherPropertyName, object? otherPropertyValue, bool notEmptyOrWhitespace = true) : ValidationAttribute
{
    /// <summary>
    /// Name of the other property.
    /// </summary>
    private string OtherPropertyName { get; } = otherPropertyName;

    /// <summary>
    /// Value of the other property when this property is required.
    /// </summary>
    private object? OtherPropertyValue { get; } = otherPropertyValue;

    /// <summary>
    /// Flag to indicate if the value should not be empty or whitespace.
    /// </summary>
    private bool NotEmptyOrWhitespace { get; } = notEmptyOrWhitespace;
    

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherPropertyName);
        
        // If the other property is not found, return an error.
        if (otherPropertyInfo is null)
            return new ValidationResult($"Unknown other property name '{OtherPropertyName}'.");
        
        // If the other property is an indexer, return an error.
        if (otherPropertyInfo.GetIndexParameters().Length > 0)
            throw new ArgumentException($"Other property not found ('{validationContext.MemberName}, '{OtherPropertyName}').");
        
        var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
        
        // If the other property is not set to the expected value, then this property is not required.
        if (!Equals(OtherPropertyValue, otherPropertyValue)) 
            return ValidationResult.Success;
        
        // If the other property is set to the expected value, then this property is required.
        if (value is null)
            return new ValidationResult($"Property '{validationContext.DisplayName}' is required when '{OtherPropertyName}' is set to '{OtherPropertyValue}'.");
        if (NotEmptyOrWhitespace && string.IsNullOrWhiteSpace(value.ToString()))
            return new ValidationResult($"Property '{validationContext.DisplayName}' cannot be empty or whitespace when '{OtherPropertyName}' is set to '{OtherPropertyValue}'.");

        return ValidationResult.Success;
    }
}
