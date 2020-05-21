using AppCore.Orm.EntityFramework.Test.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppCore.Orm.EntityFramework.Test
{
    public class EfRepositoryTest : IAsyncLifetime
    {
        private DbContextOptions<TestDbContext> _dbContextOptions;
        private TestDbContext _testDbContext;
        private TestEntityRepository _repository;
       
        public EfRepositoryTest()
        {

        }

        public Task InitializeAsync()
        {
            DbContextOptionsBuilder<TestDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            dbContextOptionsBuilder.UseInMemoryDatabase("Test");


            _dbContextOptions = dbContextOptionsBuilder.Options;
            _testDbContext = new TestDbContext(_dbContextOptions);
            _repository = new TestEntityRepository(_testDbContext);

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            if (_testDbContext != null)
            {
                _testDbContext.Database.EnsureDeleted();
            }

            return Task.CompletedTask;
        }

        [Fact]
        public async Task InsertAsync_Should_Add_Entity_To_Underlying_Database()
        {
            //Arrange
            TestEntity entity = new TestEntity();

            //Act
            await _repository.InsertAsync(entity);

            //Assert
            using (TestDbContext testDbContext = new TestDbContext(_dbContextOptions))
            {
                Assert.NotNull(testDbContext.TestEntities.FirstOrDefault());
            }
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Entity_In_Underlying_Database()
        {
            //Arrange
            TestEntity entity = new TestEntity { Id = 1, Value = "Hello World" };
            await _repository.InsertAsync(entity);
            string newValue = "Beþiktaþ";
            entity.Value = newValue;

            //Act
            await _repository.UpdateAsync(entity);

            //Assert
            using (TestDbContext testDbContext = new TestDbContext(_dbContextOptions))
            {
                Assert.True(testDbContext.TestEntities.FirstOrDefault(q => q.Id == entity.Id).Value == newValue);
            }
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Existing_Entity_From_Underlying_Database()
        {
            //Arrange
            TestEntity entity = new TestEntity { Id = 1, Value = "Hello World" };
            await _repository.InsertAsync(entity);

            //Act
            await _repository.DeleteAsync(entity);

            //Assert
            using (TestDbContext testDbContext = new TestDbContext(_dbContextOptions))
            {
                Assert.Null(testDbContext.TestEntities.FirstOrDefault(q => q.Id == entity.Id));
            }
        }

        [Fact]
        public async Task GetByIdAsync_Should_Get_Entity_When_Entity_Is_Exist_In_Underlying_Database()
        {
            //Arrange
            TestEntity entity = new TestEntity { Id = 1, Value = "Hello World" };
            await _repository.InsertAsync(entity);
            TestEntity queryResult = null;

            //Act
            using (TestDbContext testDbContext = new TestDbContext(_dbContextOptions))
            {
                queryResult = await _repository.GetByIdAsync<int>(entity.Id);
            }

            //Assert
            Assert.NotNull(queryResult);
        }
    }
}
