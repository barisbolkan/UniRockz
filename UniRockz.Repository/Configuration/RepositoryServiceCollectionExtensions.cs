using System;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using UniRockz.Repository.Asteroids;

namespace UniRockz.Repository.Configuration
{
    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
            => services
                .AddSingleton<IMongoClient>(new MongoClient(connectionString)
                    .WithReadPreference(ReadPreference.SecondaryPreferred)
                    .WithWriteConcern(WriteConcern.WMajority.With(journal: false))
                ).AddSingleton<IAsteroidRepository, AsteroidRepository>();
    }
}