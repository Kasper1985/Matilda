using System.Linq.Expressions;

namespace Matilda.WebApi.Storage.Interfaces;

/// <summary>
/// Defines the basic CRUD operations for a storage context.
/// </summary>
public interface IStorageContext<T> where T : IStorageEntity
{
    /// <summary>
    /// Query entities in the storage context.
    /// <param name="predicate">Predicate that needs to evaluate to true for a particular entry to be returned.</param>
    /// </summary>
    /// <returns><see cref="IEnumerable{T}"/> of entities that satisfies the predicate.</returns>
    // Task<IEnumerable<T>> QueryEntities(Func<T, bool> predicate);
    Task<IEnumerable<T>> QueryEntities(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Read an entity from the storage context by id.
    /// </summary>
    /// <param name="entityId">The entity id.</param>
    /// <returns>The entity with provided ID.</returns>
    Task<T?> Read(string entityId);

    /// <summary>
    /// Create an entity in the storage context.
    /// </summary>
    /// <param name="entity">The entity to be created in the context.</param>
    /// <returns>The entity saved in to storage context.</returns>
    Task<T> Create(T entity);

    /// <summary>
    /// Update/insert an entity in the storage context.
    /// </summary>
    /// <param name="entity">The entity to be updated/inserted in the context.</param>
    /// <returns>True if operation was successful, otherwise - false.</returns>
    Task<bool> Upsert(T entity);

    /// <summary>
    /// Delete an entity from the storage context.
    /// </summary>
    /// <param name="entity">The entity to be deleted from the context.</param>
    /// <returns>True if operation was successful, otherwise - false.</returns>
    Task<bool> Delete(T entity);
    
    /// <summary>
    /// Delete entities from the storage context by condition.
    /// </summary>
    /// <param name="predicate">Predicate that needs to evaluate to true for a particular entry to be deleted.</param>
    /// <returns>True if operation was successful, otherwise - false.</returns>
    Task<bool> DeleteByCondition(Expression<Func<T, bool>> predicate);
}
