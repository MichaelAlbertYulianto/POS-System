using MongoDB.Driver;

namespace Play.Common.MongoDB;

public class MongoRepository<T> : IRepository<T>
    where T : IEntity
{
    private readonly IMongoCollection<T> dbCollection;
    private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

    public MongoRepository(IMongoClient mongoClient, string databaseName, string collectionName)
    {
        IMongoDatabase database = mongoClient.GetDatabase(databaseName);
        dbCollection = database.GetCollection<T>(collectionName);
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
    }

    public async Task<T> GetAsync(Guid id)
    {
        FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
        return await dbCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        await dbCollection.InsertOneAsync(entity);

        if (entity.Id == Guid.Empty)
        {
            var filter = Builders<T>.Filter.Eq("_id", entity.Id);
            var insertedEntity = await dbCollection.Find(filter).FirstOrDefaultAsync();
            if (insertedEntity != null)
            {
                entity.Id = insertedEntity.Id;
            }
        }
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        FilterDefinition<T> filter = filterBuilder.Eq(
            existingEntity => existingEntity.Id,
            entity.Id
        );
        await dbCollection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
        await dbCollection.DeleteOneAsync(filter);
    }
}
