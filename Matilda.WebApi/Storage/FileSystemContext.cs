using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Text.Json;
using Matilda.WebApi.Storage.Interfaces;

namespace Matilda.WebApi.Storage;

/// <summary>
/// A storage context that stores entities on disk.
/// </summary>
public class FileSystemContext<T> : IStorageContext<T> where T : IStorageEntity
{
    /// <summary>
    /// The file path to store entities on disk.
    /// </summary>
    private readonly FileInfo _fileStorage;
    /// <summary>
    /// A lock object to prevent concurrent access to the file storage.
    /// </summary>
    private readonly object _fileStorageLock = new();
    /// <summary>
    /// Using a concurrent dictionary to store entities in memory.
    /// </summary>
    private readonly ConcurrentDictionary<string, T> _entities;
    
    
    /// <summary>
    /// Initializes a new instance of the OnDiskContext class and load the entities from disk.
    /// </summary>
    /// <param name="filePath">The file path to store and read entities on disk.</param>
    public FileSystemContext(FileInfo filePath)
    {
        _fileStorage = filePath;
        _entities = Load(filePath);
    }
    
    #region IStorageContext<T> implementation
    
    /// <inheritdoc/>
    public Task<IEnumerable<T>> QueryEntities(Expression<Func<T, bool>> predicate) => Task.FromResult(_entities.Values.Where(e => predicate.Compile().Invoke(e)));

    /// <inheritdoc/>
    public Task<T?> Read(string entityId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityId, nameof(entityId));
        return (_entities.TryGetValue(entityId, out var entity) ? Task.FromResult(entity) : null)!;
    }
    
    /// <inheritdoc/>
    public Task<T> Create(T entity)
    {
        entity.Id = Guid.NewGuid().ToString();

        if (_entities.TryAdd(entity.Id, entity))
        {
            Save(_entities, _fileStorage);
            return Task.FromResult(entity);
        }

        throw new  Exception("Could note create a new entity of save it to disk.");
    }

    /// <inheritdoc/>
    public Task<bool> Delete(T entity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entity.Id, nameof(entity.Id));
        
        if (_entities.TryRemove(entity.Id, out _))
        {
            Save(_entities, _fileStorage);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
    
    /// <inheritdoc/>
    public Task<bool> DeleteByCondition(Expression<Func<T, bool>> predicate)
    {
        var entitiesToRemove = _entities.Values.Where(e => predicate.Compile().Invoke(e)).ToList();
        if (entitiesToRemove.All(entity => _entities.TryRemove(entity.Id, out _)))
        {
            Save(_entities, _fileStorage);
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task<bool> Upsert(T entity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entity.Id, nameof(entity.Id));
        
        _entities.AddOrUpdate(entity.Id, entity, (_, _) => entity);
        Save(_entities, _fileStorage);

        return Task.FromResult(true);
    }

    #endregion
    
    /// <summary>
    /// Load the state of entities from disk.
    /// </summary>
    private ConcurrentDictionary<string, T> Load(FileInfo fileInfo)
    {
        lock (_fileStorageLock)
        {
            if (!fileInfo.Exists)
            {
                fileInfo.Directory!.Create();
                File.WriteAllText(fileInfo.FullName, "{}");
            }

            using var fileStream = File.Open(path: fileInfo.FullName, mode: FileMode.OpenOrCreate, access: FileAccess.Read, share: FileShare.Read);
            return JsonSerializer.Deserialize<ConcurrentDictionary<string, T>>(fileStream) ?? new ConcurrentDictionary<string, T>();
        }
    }
    
    /// <summary>
    /// Save the state of the entities to disk.
    /// </summary>
    private void Save(ConcurrentDictionary<string, T> entities, FileInfo fileInfo)
    {
        lock (_fileStorageLock)
        {
            if (!fileInfo.Exists)
            {
                fileInfo.Directory!.Create();
                File.WriteAllText(fileInfo.FullName, "{}");
            }

            using var fileStream = File.Open(path: fileInfo.FullName, mode: FileMode.OpenOrCreate, access: FileAccess.Write, share: FileShare.Read);
            JsonSerializer.Serialize(fileStream, entities);
        }
    }
}
