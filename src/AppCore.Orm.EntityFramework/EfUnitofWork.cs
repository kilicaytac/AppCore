using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm.EntityFramework
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        private IDbContextTransaction _currentTransaction;
        public EfUnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task BeginAsync(CancellationToken cancellationToken = default)
        {
            _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }
  
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            if (_dbContext != null)
                _dbContext.Dispose();

            if (_currentTransaction!=null)
                _currentTransaction.Dispose();
           
            GC.SuppressFinalize(this);
        }
    }
}
