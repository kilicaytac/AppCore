using AppCore.Orm.EntityFramework.Test.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;

namespace AppCore.Orm.EntityFramework.Test
{
    public class EfUnitOfWorkManagerTest:IAsyncLifetime
    {
        private SqliteConnection _sqliteConnection;
        private DbContextOptions<TestDbContext> _testDbContextDbOptions;

        private TestDbContext _testDbContext;
        public EfUnitOfWorkManagerTest()
        {
            
        }

        public async Task InitializeAsync()
        {
            _sqliteConnection = new SqliteConnection("DataSource=:memory:");

            _sqliteConnection.Open();

            _testDbContextDbOptions = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(_sqliteConnection)
                .Options;


            using (TestDbContext testDbContext = new TestDbContext(_testDbContextDbOptions))
            {
                await testDbContext.Database.EnsureCreatedAsync();
            }

            _testDbContext = new TestDbContext(_testDbContextDbOptions);
        }

        public async Task DisposeAsync()
        {
            await _sqliteConnection.DisposeAsync();
        }

        [Fact]
        public async Task BeginAsync_Should_Return_EfUnitOfWork_With_Active_Transaction()
        {
            //Arrange
            EfUnitOfWorkManager efUnitOfWorkManager = new EfUnitOfWorkManager(_testDbContext);

            //Act
            IUnitOfWork unitOfWork = await efUnitOfWorkManager.BeginAsync();

            //Assert
            Assert.NotNull(unitOfWork);
            Assert.NotNull(_testDbContext.Database.CurrentTransaction);
        }
    }
}
