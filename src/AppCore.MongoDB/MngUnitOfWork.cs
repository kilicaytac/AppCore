using AppCore.Orm;
using MongoDB.Driver;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.MongoDB
{
    public class MngUnitOfWork : IUnitOfWork
    {
        private readonly ITransactionOptionsAdapter _transactionOptionsAdapter;
        private readonly IMngSessionProvider _sessionProvider;

        public IClientSessionHandle Session { get { return _sessionProvider.GetSession(); } }
        public IMngSessionProvider SessionProvider { get { return _sessionProvider; } }

        public MngUnitOfWork(IMngSessionProvider sessionProvider, ITransactionOptionsAdapter transactionOptionsAdapter = null)
        {
            _sessionProvider = sessionProvider;
            _transactionOptionsAdapter = transactionOptionsAdapter;
        }

        public virtual async Task BeginAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default)
        {
            TransactionOptions transactionOptions = null;

            if (_transactionOptionsAdapter != null)
                transactionOptions = _transactionOptionsAdapter.GetTransactionOptions(isolationLevel);

            await Task.Run(() => Session.StartTransaction(transactionOptions));
        }
        public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await Session.CommitTransactionAsync(cancellationToken);
        }
        public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await Session.AbortTransactionAsync(cancellationToken);
        }
        public virtual void Dispose()
        {
            Session?.Dispose();
        }
    }
}
