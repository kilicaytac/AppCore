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
        private readonly IClientSessionHandle _session;

        public MngRepository(IMongoCollection<TEntity> collection)
        {
            _collection = collection;
        }
        public MngRepository(IMongoCollection<TEntity> collection, IClientSessionHandle session) : this(collection)
        {
            _session = session;
        }
        public MngRepository(IMongoDatabase database,string collectionName):this(database.GetCollection<TEntity>(collectionName))
        {
        }
        public MngRepository(IMongoDatabase database, string collectionName,IClientSessionHandle session) : this(database.GetCollection<TEntity>(collectionName),session)
        {
        }  

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            InsertOneOptions options = new InsertOneOptions();

            await (_session == null ? _collection.InsertOneAsync(entity, options, cancellationToken) :
                                      _collection.InsertOneAsync(_session, entity, options, cancellationToken));

            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            FindOneAndReplaceOptions<TEntity> options = new FindOneAndReplaceOptions<TEntity>();
            options.ReturnDocument = ReturnDocument.After;

            var filter = Builders<TEntity>.Filter.Eq("Id", entity.GetType().GetProperty("Id").GetValue(entity));

            return await (_session == null ? _collection.FindOneAndReplaceAsync<TEntity>(filter, entity, options, cancellationToken) :
                                             _collection.FindOneAndReplaceAsync<TEntity>(_session, filter, entity, options, cancellationToken));

        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", entity.GetType().GetProperty("Id").GetValue(entity));
            DeleteOptions options = new DeleteOptions();

            await (_session == null ? _collection.DeleteOneAsync(filter, cancellationToken) :
                                      _collection.DeleteOneAsync(_session, filter, options, cancellationToken));
        }

        public virtual async Task<TEntity> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", id);

            var findResult = await (_session == null ? _collection.FindAsync<TEntity>(filter, null, cancellationToken) :
                                                       _collection.FindAsync<TEntity>(_session, filter, null, cancellationToken));

            return findResult.SingleOrDefault();
        }
    }
}
