using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
