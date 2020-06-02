using NHibernate;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm.Nhibernate
{
    public class NhRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly INhSessionProvider _sessionProvider;
        public bool AutoFlushEnabled { get; set; }
        public virtual ISession Session { get { return _sessionProvider.GetSession(); } }

        public INhSessionProvider SessionProvider { get { return _sessionProvider; } }

        public NhRepository(INhSessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public NhRepository(INhSessionProvider sessionContext, bool autoFlushEnabled) : this(sessionContext)
        {
            this.AutoFlushEnabled = autoFlushEnabled;
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Session.SaveAsync(entity, cancellationToken);

            if (AutoFlushEnabled)
            {
                await Session.FlushAsync(cancellationToken);
            }

            return entity;
        }
        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Session.UpdateAsync(entity, cancellationToken);

            if (AutoFlushEnabled)
            {
                await Session.FlushAsync(cancellationToken);
            }

            return entity;
        }
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await Session.DeleteAsync(entity, cancellationToken);

            if (AutoFlushEnabled)
            {
                await Session.FlushAsync(cancellationToken);
            }
        }
        public virtual async Task<TEntity> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        {
            return await Session.GetAsync<TEntity>(id, cancellationToken);
        }
    }
}
