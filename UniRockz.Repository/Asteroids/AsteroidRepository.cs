using MongoDB.Driver;
using UniRockz.Repository.Asteroids.Entities;
using UniRockz.Repository.Core;

namespace UniRockz.Repository.Asteroids
{
    // Astroid collection
    internal class AsteroidRepository : MongoRepository<AsteroidInfo>, IAsteroidRepository
    {
        public AsteroidRepository(IMongoClient client)
            : base(client)
        { }
    }
}
