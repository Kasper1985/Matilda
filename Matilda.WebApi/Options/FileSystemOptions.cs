using System.ComponentModel.DataAnnotations;
using Matilda.WebApi.Attributes;

namespace Matilda.WebApi.Options;

/// <summary>
/// File system storage configuration.
/// </summary>
public class FileSystemOptions
{
    /// <summary>
    /// The file path for persistent file system storage.
    /// </summary>
    [Required, NotEmptyOrWhitespace] public string FilePath { get; init; } = string.Empty;
}
