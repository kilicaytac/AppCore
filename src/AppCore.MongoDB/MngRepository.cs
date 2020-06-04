using AppCore.Orm;
using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace AppCore.MongoDB
{
    public class MngRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IMngSessionProvider _sessionProvider;

        private readonly IMongoCollection<TEntity> _collection;
        public virtual IClientSessionHandle Session { get { return _sessionProvider.GetSession(); } }
        public IMongoCollection<TEntity> Collection { get { return _collection; } }
        public IMngSessionProvider SessionProvider { get { return _sessionProvider; } }

        public MngRepository(IMngSessionProvider sessionProvider, IMngCollectionProvider collectionProvider)
        {
            _sessionProvider = sessionProvider;
            _collection = collectionProvider.GetCollection<TEntity>();
        }

        public MngRepository(IMngSessionProvider sessionProvider, IMongoCollection<TEntity> collection)
        {
            _sessionProvider = sessionProvider;
            _collection = collection;
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(Session, entity, null, cancellationToken);

            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            FindOneAndReplaceOptions<TEntity> options = new FindOneAndReplaceOptions<TEntity>();
            options.ReturnDocument = ReturnDocument.After;

            var filter = Builders<TEntity>.Filter.Eq("Id", entity.GetType().GetProperty("Id").GetValue(entity));

            return await _collection.FindOneAndReplaceAsync<TEntity>(Session, filter, entity, options, cancellationToken);
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", entity.GetType().GetProperty("Id").GetValue(entity));
            DeleteOptions options = new DeleteOptions();

            await _collection.DeleteOneAsync(Session, filter, options, cancellationToken);
        }

        public virtual async Task<TEntity> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<TEntity>.Filter.Eq("Id", id);

            var findResult = await _collection.FindAsync<TEntity>(Session, filter, null, cancellationToken);

            return findResult.SingleOrDefault();
        }
    }
}
