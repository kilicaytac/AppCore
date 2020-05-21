using NHibernate;

namespace AppCore.Orm.Nhibernate.Test.Configuration
{
    public class TestEntityRepository : NhRepository<TestEntity>
    {
        public TestEntityRepository(ISession session) : base(session)
        {
            session.FlushMode = FlushMode.Always;
        }
    }
}
