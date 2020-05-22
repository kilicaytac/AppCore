using AppCore.Orm.EntityFramework.Test.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppCore.Orm._entityFramework.Test
{
    public class EfRepositoryTest : IAsyncLifetime
    {
        private DbContextOptions<TestDbContext> _dbContextOptions;
        private TestDbContext _testDbContext;
        private TestEntityRepository _efRepository;
        private TestEntity _entity;
       
        public EfRepositoryTest()
        {

        }

        public Task InitializeAsync()
        {
            DbContextOptionsBuilder<TestDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            dbContextOptionsBuilder.UseInMemoryDatabase("Test");


            _dbContextOptions = dbContextOptionsBuilder.Options;
            _testDbContext = new TestDbContext(_dbContextOptions);
            _efRepository = new TestEntityRepository(_testDbContext);
            _entity = new TestEntity { Id = 1, Value = "Hello World" };

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
        public async Task InsertAsync_Should_Add__entity_To_Underlying_Database()
        {
            //Arrange
          
            //Act
            await _efRepository.InsertAsync(_entity);

            //Assert
            using (TestDbContext testDbContext = new TestDbContext(_dbContextOptions))
            {
                Assert.NotNull(testDbContext.TestEntities.FirstOrDefault());
            }
        }

        [Fact]
        public async Task UpdateAsync_Should_Update__entity_In_Underlying_Database()
        {
            //Arrange
            await _efRepository.InsertAsync(_entity);
            string newValue = "Beþiktaþ";
            _entity.Value = newValue;

            //Act
            await _efRepository.UpdateAsync(_entity);

            //Assert
            using (TestDbContext testDbContext = new TestDbContext(_dbContextOptions))
            {
                Assert.True(testDbContext.TestEntities.FirstOrDefault(q => q.Id == _entity.Id).Value == newValue);
            }
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Existing__entity_From_Underlying_Database()
        {
            //Arrange
            await _efRepository.InsertAsync(_entity);

            //Act
            await _efRepository.DeleteAsync(_entity);

            //Assert
            using (TestDbContext testDbContext = new TestDbContext(_dbContextOptions))
            {
                Assert.Null(testDbContext.TestEntities.FirstOrDefault(q => q.Id == _entity.Id));
            }
        }

        [Fact]
        public async Task GetByIdAsync_Should_Get__entity_When__entity_Is_Exist_In_Underlying_Database()
        {
            //Arrange
            await _efRepository.InsertAsync(_entity);
            TestEntity queryResult = null;

            //Act
            using (TestDbContext testDbContext = new TestDbContext(_dbContextOptions))
            {
                queryResult = await _efRepository.GetByIdAsync<int>(_entity.Id);
            }

            //Assert
            Assert.NotNull(queryResult);
        }
    }
}
