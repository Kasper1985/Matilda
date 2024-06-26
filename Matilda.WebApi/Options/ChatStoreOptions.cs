using Matilda.WebApi.Attributes;

namespace Matilda.WebApi.Options;

public class ChatStoreOptions
{
    public const string SectionName = "ChatStore";

    public string Type { get; init; } = string.Empty;
    
    /// <summary>
    /// Configuration for the file system chat store.
    /// </summary>
    [RequiredOnPropertyValue(nameof(Type), "Filesystem")] public FileSystemOptions? Filesystem { get; init; }
    
    /// <summary>
    /// Configuration for the MongoDB chat store.
    /// </summary>
    [RequiredOnPropertyValue(nameof(Type), "MongoDB")] public MongoDbOptions? MongoDb { get; init; }
}
