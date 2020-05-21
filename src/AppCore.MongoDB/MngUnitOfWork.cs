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
        private readonly MongoClient _client;
        private IClientSessionHandle _session;
        private readonly TransactionContext _transactionContext;
        public MngUnitOfWork(MongoClient client, TransactionContext transactionContext)
        {
            _client = client;
            _transactionContext = transactionContext;
        }
        public async Task BeginAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default)
        {
            ClientSessionOptions sessionOptions = new ClientSessionOptions();

            _session = await _client.StartSessionAsync(sessionOptions, cancellationToken);
            _transactionContext.Session = _session;
            _session.StartTransaction();
        }
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _session.CommitTransactionAsync(cancellationToken);
        }
        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
