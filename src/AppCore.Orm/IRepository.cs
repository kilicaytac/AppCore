using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default);
    }
}
