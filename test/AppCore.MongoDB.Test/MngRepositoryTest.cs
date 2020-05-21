using AppCore.MongoDB.Test.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;
using Xunit;

namespace AppCore.MongoDB.Test
{
    [Collection("MngInstanceCollection")]
    public class MngRepositoryTest : IAsyncLifetime
    {
        private readonly MongoClient _mongoClient;
        private IMongoDatabase _database;
        private IMongoCollection<TestEntity> _collection;
        private MngRepository<TestEntity> _repository;
        public MngRepositoryTest(MngInstanceFixture mngInstanceFixture)
        {
            _mongoClient = mngInstanceFixture.MongoClient;
        }

        public Task InitializeAsync()
        {
            _database = _mongoClient.GetDatabase("Test");
            _database.CreateCollection("TestEntities");
            _collection = _database.GetCollection<TestEntity>("TestEntities");
            _repository = new MngRepository<TestEntity>(_collection,new TransactionContext());

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _mongoClient.DropDatabaseAsync("Test");
        }

        [Fact]
        public async Task InsertAsync_Should_Add_Entity_To_Underlying_Database()
        {
            //Arrange
            TestEntity entity = new TestEntity();

            //Act
            await _repository.InsertAsync(entity);

            //Assert
            var filter = Builders<TestEntity>.Filter.Eq("Id", entity.Id);
            Assert.NotNull(_collection.Find(filter).SingleOrDefault());
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
            TestEntity result = await _repository.UpdateAsync(entity);

            //Assert
            var filter = Builders<TestEntity>.Filter.Eq("Id", entity.Id);
            Assert.True(_collection.Find<TestEntity>(filter).SingleOrDefault().Value == newValue);
            Assert.True(result.Value == newValue);
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
            var filter = Builders<TestEntity>.Filter.Eq("Id", entity.Id);
            Assert.Null(_collection.Find(filter).SingleOrDefault());
        }

        [Fact]
        public async Task GetByIdAsync_Should_Get_Entity_When_Entity_Is_Exist_In_Underlying_Database()
        {
            //Arrange
            TestEntity entity = new TestEntity { Id = 1, Value = "Hello World" };
            await _repository.InsertAsync(entity);
            TestEntity queryResult = null;

            //Act
            queryResult = await _repository.GetByIdAsync<int>(entity.Id);

            //Assert
            Assert.NotNull(queryResult);
        }
    }
}
