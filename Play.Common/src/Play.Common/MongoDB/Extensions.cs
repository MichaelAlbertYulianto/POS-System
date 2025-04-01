using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB;

public static class Extensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            var mongoDbSettings = new MongoDbSettings();
            configuration.GetSection(nameof(MongoDbSettings)).Bind(mongoDbSettings);
            var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
            return mongoClient;
        });

        return services;
    }

    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName)
        where T : IEntity
    {
        services.AddSingleton<IRepository<T>>(serviceProvider =>
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            var mongoDbSettings = new MongoDbSettings();
            configuration.GetSection(nameof(MongoDbSettings)).Bind(mongoDbSettings);
            var mongoClient = serviceProvider.GetService<IMongoClient>();
            return new MongoRepository<T>(mongoClient, mongoDbSettings.DatabaseName, collectionName);
        });

        return services;
    }
}
