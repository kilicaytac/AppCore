using NHibernate;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm.Nhibernate
{
    public class NhRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ISession _session;
        public ISession Session { get { return _session; } }
        public NhRepository(ISession session)
        {
            _session = session;
        }
        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _session.SaveAsync(entity,cancellationToken);

            return entity;
        }
        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _session.UpdateAsync(entity,cancellationToken);

            return entity;
        }
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _session.DeleteAsync(entity,cancellationToken);
        }
        public virtual async Task<TEntity> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        {
            return await _session.GetAsync<TEntity>(id, cancellationToken);
        }
    }
}
