using System.Linq.Expressions;

namespace Matilda.WebApi.Storage.Interfaces;

/// <summary>
/// Defines the basic CRUD operations for a repository.
/// </summary>
public interface IRepository<T> where T : IStorageEntity
{
    /// <summary>
    /// Creates a new entity in the repository.
    /// </summary>
    /// <param name="entity">An entity of type T.</param>
    /// <returns>The created entity.</returns>
    Task<T> Create(T entity);

    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>True if the entity was deleted, false otherwise.</returns>
    Task<bool> Delete(T entity);

    /// <summary>
    /// Updates/inserts an entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to be updated/inserted.</param>
    Task<bool> Upsert(T entity);

    /// <summary>
    /// Finds an entity by its id.
    /// </summary>
    /// <param name="id">ID of the entity.</param>
    /// <returns>An entity</returns>
    Task<T> FindById(string id);

    /// <summary>
    /// Tries to find an entity by its id.
    /// </summary>
    /// <param name="id">ID of the entity.</param>
    /// <param name="callback">The entity delegate. Note async methods don't support ref or out parameters.</param>
    /// <returns>True if the entity was found, false otherwise.</returns>
    Task<bool> TryFindById(string id, Action<T?> callback);

    /// <summary>
    /// Finds entities that match a predicate.
    /// </summary>
    /// <param name="predicate">Predicate that needs to evaluate to true for a particular entity to be returned.</param>
    /// <param name="skip">Number of entities to skip before starting to return entities.</param>
    /// <param name="count">Number of entities to return. -1 returns all entities.</param>
    /// <returns><see cref="IEnumerable{T}"/> of entities matching the given predicate.</returns>
    Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, int skip = 0, int count = -1);
}
