using NHibernate;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm.Nhibernate
{
    public class NhUnitOfWork : IUnitOfWork
    {
        private readonly ISession _session;
        private ITransaction _currentTransaction;
        public NhUnitOfWork(ISession session)
        {
            _session = session;
        }
        public async Task BeginAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null && _currentTransaction.IsActive)
                throw new Exception("There is already an open transaction.");

            await Task.Run(() => _currentTransaction = _session.BeginTransaction(isolationLevel));
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _session.FlushAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            if (_currentTransaction != null)
                _currentTransaction.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
