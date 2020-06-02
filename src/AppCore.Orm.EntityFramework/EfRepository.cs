using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm.EntityFramework
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;

        private readonly DbSet<TEntity> _dbSet;
        public bool AutoFlushEnabled { get; set; }
        public DbContext DbContext { get { return _dbContext; } }
        public DbSet<TEntity> DbSet { get { return _dbSet; } }

        public EfRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public EfRepository(DbContext dbContext, bool autoFlushEnabled) : this(dbContext)
        {
            AutoFlushEnabled = autoFlushEnabled;
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Add(entity);

            if (AutoFlushEnabled)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return entity;
        }
        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;

            if (AutoFlushEnabled)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return entity;
        }
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;

            if (AutoFlushEnabled)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
        public virtual async Task<TEntity> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }
    }
}
