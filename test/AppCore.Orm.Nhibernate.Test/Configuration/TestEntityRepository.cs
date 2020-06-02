using NHibernate;

namespace AppCore.Orm.Nhibernate.Test.Configuration
{
    public class TestEntityRepository : NhRepository<TestEntity>
    {
        public TestEntityRepository(INhSessionProvider sessionContext) : base(sessionContext)
        {

        }
    }
}
