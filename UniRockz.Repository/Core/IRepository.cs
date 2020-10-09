using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace UniRockz.Repository.Core
{
    public interface IRepository<TEntity> where TEntity : IDocument
    {
        IEnumerable<TEntity> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken);

        TEntity GetById(string id);
        Task<TEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
        IQueryable<TEntity> GetAllAsync2(CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> BulkUpsert(IEnumerable<TEntity> items);
    }
}
