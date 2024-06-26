using System.Linq.Expressions;
using Matilda.WebApi.Storage.Interfaces;

namespace Matilda.WebApi.Storage;

/// <summary>
/// Defines the basic CRUD operations for a repository.
/// </summary>
/// <param name="storageContext">The storage context.</param>
public class Repository<T>(IStorageContext<T> storageContext) : IRepository<T> where T : IStorageEntity
{
    /// <inheritdoc/>
    public Task<T> Create(T entity) => storageContext.Create(entity);

    /// <inheritdoc/>
    public Task<bool> Delete(T entity) => storageContext.Delete(entity);

    /// <inheritdoc/>
    public Task<T> FindById(string id) => storageContext.Read(id);

    /// <inheritdoc/>
    public async Task<bool> TryFindById(string id, Action<T?>? callback = null)
    {
        try
        {
            var found = await FindById(id);
            callback?.Invoke(found);
            return true;
        }
        catch (Exception ex) when (ex is ArgumentOutOfRangeException or KeyNotFoundException)
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public Task<bool> Upsert(T entity) => storageContext.Upsert(entity);

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate, int skip = 0, int count = -1)
    {
        var entities = await storageContext.QueryEntities(predicate);
        return count < 0 ? entities.Skip(skip) : entities.Skip(skip).Take(count);
    }
}
