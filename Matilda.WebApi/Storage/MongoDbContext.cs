using System.Linq.Expressions;
using Matilda.WebApi.Storage.Interfaces;
using MongoDB.Driver;

namespace Matilda.WebApi.Storage;

public class MongoDbContext<T> : IStorageContext<T> where T : IStorageEntity
{
    private readonly IMongoCollection<T> _collection;
    
    public MongoDbContext(string connectionString, string database)
    {
        var settings = MongoClientSettings.FromConnectionString(connectionString);
        var client = new MongoClient(settings);
        _collection = client.GetDatabase(database).GetCollection<T>(typeof(T).Name);
    }

    #region IStorageContext<T> implementation
    
    /// <inheritdoc/>
    public async Task<IEnumerable<T>> QueryEntities(Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(_collection);
        
        return await _collection.Find(predicate).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<T?> Read(string entityId)
    {
        ArgumentNullException.ThrowIfNull(_collection);
        ArgumentException.ThrowIfNullOrWhiteSpace(entityId, nameof(entityId));
        
        return await _collection.Find(e => e.Id == entityId).FirstOrDefaultAsync();        
    }

    /// <inheritdoc/>
    public async Task<T> Create(T entity)
    {
        ArgumentNullException.ThrowIfNull(_collection);
        
        entity.Id = Guid.NewGuid().ToString();
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    /// <inheritdoc/>
    public async Task<bool> Upsert(T entity)
    {
        ArgumentNullException.ThrowIfNull(_collection);
        ArgumentException.ThrowIfNullOrWhiteSpace(entity.Id, nameof(entity.Id));
        
        var result = await _collection.ReplaceOneAsync(e => e.Id == entity.Id, entity, new ReplaceOptions { IsUpsert = true });
        return result?.IsAcknowledged ?? false;
    }

    /// <inheritdoc/>
    public async Task<bool> Delete(T entity)
    {
        ArgumentNullException.ThrowIfNull(_collection);
        ArgumentException.ThrowIfNullOrWhiteSpace(entity.Id, nameof(entity.Id));
        
        var result = await _collection.DeleteOneAsync(e => e.Id == entity.Id);
        return result?.IsAcknowledged ?? false;
    }
    
    /// <inheritdoc/>
    public async Task<bool> DeleteByCondition(Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(_collection);
        
        var result = await _collection.DeleteManyAsync(predicate);
        return result?.IsAcknowledged ?? false;
    }

    #endregion
}
