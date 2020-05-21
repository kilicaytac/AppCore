using AppCore.Orm;
using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace AppCore.MongoDB
{
    public class MngRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IMongoCollection<TEntity> _collection;
        private readonly TransactionContext _transactionContext;
        public MngRepository(IMongoCollection<TEntity> collection, TransactionContext transactionContext)
        {
            _collection = collection;
            _transactionContext = transactionContext;
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            InsertOneOptions options = new InsertOneOptions();

            await (_transactionContext.Session == null ? _collection.InsertOneAsync(entity, options, cancellationToken) :
                                      _collection.InsertOneAsync(_transactionContext.Session, entity, options, cancellationToken));

            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            FindOneAndReplaceOptions<TEntity> options = new FindOneAndReplaceOptions<TEntity>();
            options.ReturnDocument = ReturnDocument.After;

            var filter = Builders<TEntity>.Filter.Eq("Id", entity.GetType().GetProperty("Id").GetValue(entity));

            return await (_transactionContext.Session == null ? _collection.FindOneAndReplaceAsync<TEntity>(filter, entity, options, cancellationToken) :
                                             _collection.FindOneAndReplaceAsync<TEntity>(_transactionContext.Session, filter, entity, options, cancellationToken));

        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", entity.GetType().GetProperty("Id").GetValue(entity));
            DeleteOptions options = new DeleteOptions();

            await (_transactionContext.Session == null ? _collection.DeleteOneAsync(filter, cancellationToken) :
                                      _collection.DeleteOneAsync(_transactionContext.Session, filter, options, cancellationToken));
        }

        public virtual async Task<TEntity> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", id);

            var findResult = await (_transactionContext.Session == null ? _collection.FindAsync<TEntity>(filter, null, cancellationToken) :
                                                       _collection.FindAsync<TEntity>(_transactionContext.Session, filter, null, cancellationToken));

            return findResult.SingleOrDefault();
        }
    }
}
