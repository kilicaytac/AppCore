using AppCore.Orm;
using MongoDB.Driver;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore.MongoDB
{
    public class MngUnitOfWork : IUnitOfWork
    {
        private readonly IClientSessionHandle _session;
        private readonly ITransactionOptionsConverter _transactionOptionsConverter;
        public MngUnitOfWork(IClientSessionHandle session)
        {
            _session = session;
        }

        public MngUnitOfWork(IClientSessionHandle session, ITransactionOptionsConverter transactionOptionsConverter) : this(session)
        {
            _transactionOptionsConverter = transactionOptionsConverter;
        }
        public virtual async Task BeginAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default)
        {
            TransactionOptions transactionOptions = null;

            if (_transactionOptionsConverter != null)
                transactionOptions = _transactionOptionsConverter.ConvertFromIsolationLevel(isolationLevel);
            else
                transactionOptions = isolationLevel.ToMngTransactionOptions();

            await Task.Run(() => _session.StartTransaction(transactionOptions));
        }
        public virtual async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _session.CommitTransactionAsync(cancellationToken);
        }
        public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await _session.AbortTransactionAsync(cancellationToken);
        }
        public virtual void Dispose()
        {
            _session?.Dispose();
        }
    }
}
