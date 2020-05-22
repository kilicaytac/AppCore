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
        public MngUnitOfWork(IClientSessionHandle session)
        {
            _session = session;
        }
        public async Task BeginAsync(IsolationLevel isolationLevel = IsolationLevel.Unspecified, CancellationToken cancellationToken = default)
        {
            //isolation level to => read,write concerns
            await Task.Run(() => _session.StartTransaction());
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
            _session?.Dispose();
        }
    }
}
