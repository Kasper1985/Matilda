namespace Matilda.WebApi.Storage.Interfaces;

/// <summary>
/// Defines the main properties of a storage entity.
/// </summary>
public interface IStorageEntity
{
    /// <summary>
    /// Unique ID of the entity.
    /// </summary>
    string Id { get; set; }
}
