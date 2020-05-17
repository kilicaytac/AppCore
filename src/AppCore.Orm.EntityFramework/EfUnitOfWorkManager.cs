using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm.EntityFramework
{
    public class EfUnitOfWorkManager: IUnitOfManager
    {
        private readonly DbContext _dbContext;
        public EfUnitOfWorkManager(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IUnitOfWork> BeginAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default)
        {
            EfUnitOfWork unitOfWork = new EfUnitOfWork(_dbContext);
            await unitOfWork.BeginAsync(isolationLevel,cancellationToken);

            return unitOfWork;
        }
    }
}
