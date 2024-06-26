using Matilda.WebApi.Storage.Interfaces;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel.ChatCompletion;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Matilda.WebApi.Models;

public class ChatMessage : IStorageEntity
{
    /// <summary>
    /// ID of the chat message.
    /// </summary>
    [BsonId]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Role of the author of a chat message.
    /// </summary>
    public AuthorRole Role { get; set; }
    
    /// <summary>
    /// Timestamp of the message.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset TimeStamp { get; set; }
    
    /// <summary>
    /// Content of the message.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// Citations of the message.
    /// </summary>
    public IEnumerable<Citation>? Citations { get; set; }
    
    /// <summary>
    /// Counts of total token usage used to generate response.
    /// </summary>
    public IDictionary<string, int>? TokenUsage { get; set; }
}
