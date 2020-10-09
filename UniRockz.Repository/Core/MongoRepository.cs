using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using UniRockz.Repository.Attributes;

namespace UniRockz.Repository.Core
{
    internal abstract class MongoRepository<TEntity> : IRepository<TEntity> where TEntity : IDocument
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<TEntity> _collection;

        public MongoRepository(IMongoClient client)
        {
            _client = client;

            var info = (CollectionInfoAttribute)((typeof(TEntity)
                .GetCustomAttributes(typeof(CollectionInfoAttribute), true)).FirstOrDefault());
            _database = _client.GetDatabase(info.DatabaseName);
            _collection = _database.GetCollection<TEntity>(info.CollectionName);
        }

        public async Task<IEnumerable<TEntity>> BulkUpsert(IEnumerable<TEntity> items)
        {
            var result = await _collection.BulkWriteAsync(
                items.Select(i => new ReplaceOneModel<TEntity>(Builders<TEntity>.Filter.Eq("_id", i.Id), i)
                {
                    IsUpsert = true
                }), new BulkWriteOptions() { IsOrdered = false });
            var inserted = result.Upserts.Select(u => u.Id.AsString);

            return items.Where(i => !inserted.Contains(i.Id));
        }

        public IEnumerable<TEntity> GetAll() =>
             _collection
                .Find<TEntity>(Builders<TEntity>.Filter.Empty)
                .ToEnumerable();

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken)
        {
            return await _collection
                .Find(Builders<TEntity>.Filter.Where(filter))
                .ToListAsync<TEntity>(cancellationToken);
        }

        public IQueryable<TEntity> GetAllAsync2(CancellationToken cancellationToken)
        {
            return _collection.AsQueryable();
        }

        public TEntity GetById(string id) =>
            _collection
                .Find(Builders<TEntity>.Filter.Eq("_id", new ObjectId(id)))
                .FirstOrDefault();

        public async Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken) =>
             await _collection
                .Find(Builders<TEntity>.Filter.Eq("_id", new ObjectId(id)))
                .FirstOrDefaultAsync(cancellationToken);
    }

    static class DeconstructionExtensions
    {
        public static void Deconstruct<T>(this BulkWriteResult<T> result, out long modified, out IEnumerable<ObjectId> inserted)
        {
            modified = result.ModifiedCount;
            inserted = result.Upserts.Select(f => f.Id.AsObjectId);
        }
    }
}
