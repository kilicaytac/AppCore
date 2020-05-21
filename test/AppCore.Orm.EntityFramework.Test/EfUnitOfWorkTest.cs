using AppCore.Orm.EntityFramework.Test.Configuration;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace AppCore.Orm.EntityFramework.Test
{
    public class EfUnitOfWorkTest : IAsyncLifetime
    {
        private SqliteConnection _sqliteConnection;
        private DbContextOptions<TestDbContext> _testDbContextDbOptions;

        private TestDbContext _testDbContext;
        private EfUnitOfWork _efUnitofWork;
        private EfRepository<TestEntity> _repository;
        public EfUnitOfWorkTest()
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
            _repository = new EfRepository<TestEntity>(_testDbContext);
            _efUnitofWork = new EfUnitOfWork(_testDbContext);
        }

        public async Task DisposeAsync()
        {
            await _sqliteConnection.DisposeAsync();
        }

        [Fact]
        public async Task BeginAsync_Should_Open_Transaction_To_Underlying_Database()
        {
            //Arrange

            //Act
            await _efUnitofWork.BeginAsync();

            //Assert
            Assert.NotNull(_testDbContext.Database.CurrentTransaction);
        }

        [Fact]
        public async Task BeginAsync_Should_Throw_Exception_When_Already_Has_Active_Transaction()
        {
            //Arrange
            await _efUnitofWork.BeginAsync();
            //Act
            //Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _efUnitofWork.BeginAsync());
        }

        [Fact]
        public async Task CommitAsync_Should_Commit_Transaction_Changes_To_Underlying_Database()
        {
            //Arrange
            await _efUnitofWork.BeginAsync();
            TestEntity testEntity = new TestEntity { Id = 1, Value = "Beşiktaş" };
            await _repository.InsertAsync(testEntity);

            //Act
            await _efUnitofWork.CommitAsync();

            //Assert
            using (TestDbContext newTestDbContext = new TestDbContext(_testDbContextDbOptions))
            {
                Assert.NotNull(newTestDbContext.TestEntities.FirstOrDefault(q => q.Id == testEntity.Id));
            }
        }

        [Fact]
        public async Task RollbackAsync_Should_Rollback_Uncommitted_Changes_To_Underlying_Database()
        {
            //Arrange
            await _efUnitofWork.BeginAsync();
            TestEntity testEntity = new TestEntity { Id = 1, Value = "Beşiktaş" };
            await _repository.InsertAsync(testEntity);

            //Act
            await _efUnitofWork.RollbackAsync();

            //Assert
            using (TestDbContext newTestDbContext = new TestDbContext(_testDbContextDbOptions))
            {
                Assert.Null(newTestDbContext.TestEntities.FirstOrDefault(q => q.Id == testEntity.Id));
            }
        }

        [Fact]
        public async Task Should_Begin_Another_Transaction_After_Previous_Transaction_Committed()
        {
            //Arrange
            await _efUnitofWork.BeginAsync();
            TestEntity testEntity = new TestEntity { Id = 1, Value = "Beşiktaş" };
            await _repository.InsertAsync(testEntity);
            await _efUnitofWork.CommitAsync();

            //Act
            await _efUnitofWork.BeginAsync();
            TestEntity testEntity2 = new TestEntity { Id = 2, Value = "Beşiktaşş" };
            await _repository.InsertAsync(testEntity2);
            await _efUnitofWork.CommitAsync();

            //Assert
            using (TestDbContext newTestDbContext = new TestDbContext(_testDbContextDbOptions))
            {
                Assert.True(newTestDbContext.TestEntities.Count() == 2);
            }
        }

        [Fact]
        public async Task Should_Get_Uncommited_Entity_When_BeginAsync_Called_With_ReadUncommitted_Isolation_Level()
        {
            //Arrange
            await _efUnitofWork.BeginAsync(System.Data.IsolationLevel.ReadUncommitted);

            TestEntity testEntity = new TestEntity { Id = 1, Value = "Beşiktaş" };
            await _repository.InsertAsync(testEntity);

            //Act
            TestEntity result = await new EfRepository<TestEntity>(_testDbContext).GetByIdAsync<int>(testEntity.Id);

            //Assert
            Assert.NotNull(result);
        }
    }
}
