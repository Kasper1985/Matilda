using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using Matilda.WebApi.Storage.Interfaces;

namespace Matilda.WebApi.Storage;

/// <summary>
/// A storage context that stores entities in memory.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class InMemoryContext<T> : IStorageContext<T> where T : IStorageEntity
{
    // Using a concurrent dictionary to store entities in memory.
    private readonly ConcurrentDictionary<string, T> _entities;
    
    
    /// <summary>
    /// Initialized a new instance of the InMemoryContext class.
    /// </summary>
    public InMemoryContext()
    {
        _entities = new ConcurrentDictionary<string, T>();
    }

    #region ISotrageContext<T> implementation
    
    /// <inheritdoc/>
    public Task<IEnumerable<T>> QueryEntities(Expression<Func<T, bool>> predicate) => Task.FromResult(_entities.Values.Where(e => predicate.Compile().Invoke(e)));
    
    /// <inheritdoc/>
    public Task<T> Read(string entityId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityId, nameof(entityId));
        return _entities.TryGetValue(entityId, out var entity) ? Task.FromResult(entity) : throw new KeyNotFoundException($"Entity with ID '{entityId}' not found.");
    }
    
    /// <inheritdoc/>
    public Task<T> Create(T entity)
    {
        entity.Id = Guid.NewGuid().ToString();
        return _entities.TryAdd(entity.Id, entity) ? Task.FromResult(entity) : throw new Exception("Could not create a new entity.");
    }
    
    /// <inheritdoc/>
    public Task<bool> Delete(T entity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entity.Id, nameof(entity.Id));
        return Task.FromResult(_entities.TryRemove(entity.Id, out _));
    }

    /// <inheritdoc/>
    public Task<bool> Upsert(T entity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entity.Id, nameof(entity.Id));
        _entities.AddOrUpdate(entity.Id, entity, (_, _) => entity);
        return Task.FromResult(true);
    }
    
    #endregion
    
    private string GetDebuggerDisplay() => ToString() ?? string.Empty;
}
