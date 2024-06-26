using System.ComponentModel.DataAnnotations;
using Matilda.WebApi.Attributes;

namespace Matilda.WebApi.Options;

/// <summary>
/// MongoDB configuration.
/// </summary>
public class MongoDbOptions
{
    /// <summary>
    /// Connection string to the MongoDB.
    /// </summary>
    [Required, NotEmptyOrWhitespace] public string ConnectionString { get; init; } = string.Empty;
    
    /// <summary>
    /// Name of database for the MongoDB.
    /// </summary>
    [Required, NotEmptyOrWhitespace] public string DatabaseName { get; init; } = string.Empty;
}
