using AppCore.Orm.EntityFramework.Test.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppCore.Orm.EntityFramework.Test
{
    public class EfRepositoryTest : IAsyncLifetime
    {
        private TestDbContext _testDbContext;
        private EfRepository<TestEntity> _repository;

        public EfRepositoryTest()
        {

        }

        public Task InitializeAsync()
        {
            DbContextOptionsBuilder<TestDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            dbContextOptionsBuilder.UseInMemoryDatabase("Test");

            _testDbContext = new TestDbContext(dbContextOptionsBuilder.Options);
            _repository = new EfRepository<TestEntity>(_testDbContext);

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
        public void Add_Should_Add_Entity_To_Related_DbSet_Of_DbContext()
        {
            //Arrange
            TestEntity entity = new TestEntity();

            //Act
            _repository.Add(entity);
            _testDbContext.SaveChanges();

            //Assert
            Assert.NotNull(_testDbContext.TestEntities.FirstOrDefault());
        }

        [Fact]
        public void Update_Should_Update_Entity_In_Related_DbSet_Of_DbContext()
        {
            //Arrange
            TestEntity entity = new TestEntity { Id = 1, Value = "Hello World" };

            _repository.Add(entity);
            _testDbContext.SaveChanges();

            string newValue = "Beþiktaþ";

            entity.Value = newValue;

            //Act
            _repository.Update(entity);
            _testDbContext.SaveChanges();

            //Assert
            Assert.True(_testDbContext.TestEntities.FirstOrDefault(q => q.Id == entity.Id).Value == newValue);
        }


        [Fact]
        public void Delete_Should_Remove_Entity_From_Related_DbSet_Of_DbContext()
        {
            //Arrange
            TestEntity entity = new TestEntity { Id = 1, Value = "Hello World" };

            _repository.Add(entity);
            _testDbContext.SaveChanges();

            //Act
            _repository.Delete(entity);
            _testDbContext.SaveChanges();

            //Assert
            Assert.Null(_testDbContext.TestEntities.FirstOrDefault(q => q.Id == entity.Id));
        }


        [Fact]
        public async Task SaveChangesAsync_Should_Commit_Changes_To_Underlying_Database()
        {
            //Arrange
            TestEntity entity = new TestEntity { Id = 1, Value = "Hello World" };
            _repository.Add(entity);

            //Act
            await _repository.SaveChangesAsync();

            //Assert
            Assert.NotNull(_testDbContext.TestEntities.FirstOrDefault(q => q.Id == entity.Id));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Get_Entity_When_Entity_Is_Exist_In_Related_DbSet_Of_DbContext()
        {
            //Arrange
            TestEntity entity = new TestEntity { Id = 1, Value = "Hello World" };
            _repository.Add(entity);
            await _repository.SaveChangesAsync();

            //Act
            var queryResult = await _repository.GetByIdAsync<int>(entity.Id);

            //Assert
            Assert.NotNull(queryResult);
        }
    }
}
