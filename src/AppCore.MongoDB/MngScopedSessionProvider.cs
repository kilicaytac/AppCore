using System;
using System.Threading;
using MongoDB.Driver;

namespace AppCore.MongoDB
{
    public class MngScopedSessionProvider : IMngSessionProvider, IDisposable
    {
        private IClientSessionHandle _session;
        private readonly IMongoClient _client;
        private readonly ClientSessionOptions _sessionOptions;
        private readonly CancellationToken _cancellationToken;
        public IMongoClient Client { get { return _client; } }

        public MngScopedSessionProvider(IMongoClient client, ClientSessionOptions sessionOptions = null, CancellationToken cancellationToken = default)
        {
            _client = client;
            _sessionOptions = sessionOptions;
            _cancellationToken = cancellationToken;
        }

        public virtual IClientSessionHandle GetSession()
        {
            if (_session == null)
            {
                _session = _client.StartSession(_sessionOptions, _cancellationToken);
            }

            return _session;
        }
        public virtual void Dispose()
        {
            _session?.Dispose();
        }
    }
}
