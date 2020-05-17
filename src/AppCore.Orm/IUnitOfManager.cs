using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm
{
    public interface IUnitOfManager
    {
        Task<IUnitOfWork> BeginAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default);
    }
}
