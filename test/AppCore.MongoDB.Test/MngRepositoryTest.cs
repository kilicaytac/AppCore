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
        private MngRepository<TestEntity> _mngRepository;
        private TestEntity _entity;
        public MngRepositoryTest(MngInstanceFixture mngInstanceFixture)
        {
            _mongoClient = mngInstanceFixture.MongoClient;
        }

        public Task InitializeAsync()
        {
            _database = _mongoClient.GetDatabase("Test");
            _collection = _database.GetCollection<TestEntity>("TestEntities");
            _mngRepository = new MngRepository<TestEntity>(_collection);
            _entity = new TestEntity { Id = 1, Value = "Hello World" };

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

            //Act
            await _mngRepository.InsertAsync(_entity);

            //Assert
            var filter = Builders<TestEntity>.Filter.Eq("Id", _entity.Id);
            Assert.NotNull(_collection.Find(filter).SingleOrDefault());
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Entity_In_Underlying_Database()
        {
            //Arrange
            await _mngRepository.InsertAsync(_entity);
            string newValue = "Beþiktaþ";
            _entity.Value = newValue;

            //Act
            TestEntity result = await _mngRepository.UpdateAsync(_entity);

            //Assert
            var filter = Builders<TestEntity>.Filter.Eq("Id", _entity.Id);
            Assert.True(_collection.Find<TestEntity>(filter).SingleOrDefault().Value == newValue);
            Assert.True(result.Value == newValue);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Existing_Entity_From_Underlying_Database()
        {
            //Arrange
            await _mngRepository.InsertAsync(_entity);

            //Act
            await _mngRepository.DeleteAsync(_entity);

            //Assert
            var filter = Builders<TestEntity>.Filter.Eq("Id", _entity.Id);
            Assert.Null(_collection.Find(filter).SingleOrDefault());
        }

        [Fact]
        public async Task GetByIdAsync_Should_Get_Entity_When_Entity_Is_Exist_In_Underlying_Database()
        {
            //Arrange
            await _mngRepository.InsertAsync(_entity);

            //Act
            TestEntity queryResult = await _mngRepository.GetByIdAsync<int>(_entity.Id);

            //Assert
            Assert.NotNull(queryResult);
        }
    }
}
