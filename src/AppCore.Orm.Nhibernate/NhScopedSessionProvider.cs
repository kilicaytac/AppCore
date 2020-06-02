using System;
using NHibernate;

namespace AppCore.Orm.Nhibernate
{
    public class NhScopedSessionProvider : INhSessionProvider, IDisposable
    {
        private ISession _session;

        private readonly FlushMode _flushMode;

        private readonly ISessionFactory _sessionFactory;
        public FlushMode FlushMode { get { return _flushMode; } }
        public ISessionFactory SessionFactory { get { return _sessionFactory; } }
        public NhScopedSessionProvider(ISessionFactory sessionFactory, FlushMode flushMode = default)
        {
            _sessionFactory = sessionFactory;
            _flushMode = flushMode;
        }

        public virtual ISession GetSession()
        {
            if (_session == null)
            {
                _session = _sessionFactory.OpenSession();
                _session.FlushMode = _flushMode;
            }

            return _session;
        }

        public virtual void Dispose()
        {
            _session?.Dispose();
        }
    }
}
