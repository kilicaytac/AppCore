using NHibernate;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm.Nhibernate
{
    public class NhUnitOfWork : IUnitOfWork
    {
        private readonly INhSessionProvider _sessionProvider;

        private ITransaction _currentTransaction;
        public bool AutoFlushEnabled { get; set; }
        public virtual ISession Session { get { return _sessionProvider.GetSession(); } }

        public NhUnitOfWork(INhSessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }
        public virtual async Task BeginAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null && _currentTransaction.IsActive)
                throw new Exception("There is already an open transaction.");

            await Task.Run(() => _currentTransaction = Session.BeginTransaction(isolationLevel));
        }
        public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await Session.FlushAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }

        public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }

        public virtual void Dispose()
        {
            _currentTransaction?.Dispose();
        }
    }
}
