using System.Threading;
using System.Threading.Tasks;

namespace AppCore.Orm.EntityFramework.Test.Configuration
{
    public class TestEntityRepository : EfRepository<TestEntity>
    {
        public TestEntityRepository(TestDbContext dbContext) : base(dbContext, true)
        {
        }
    }
}
