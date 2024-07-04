using Matilda.WebApi.Storage.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Matilda.WebApi.Models;

public class Chat : IStorageEntity
{
    /// <summary>
    /// ID of the chat history
    /// </summary>
    [BsonId]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Chat title
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Timestamp of the chat creation
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset TimeStamp { get; set; }
}
