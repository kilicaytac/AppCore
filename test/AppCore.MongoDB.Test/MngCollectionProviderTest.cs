using AppCore.MongoDB.Test.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;
using Xunit;

namespace AppCore.MongoDB.Test
{
    [Collection("MngInstanceCollection")]
    public class MngCollectionProviderTest: IAsyncLifetime
    {
        private readonly MongoClient _mongoClient;
        private IMongoDatabase _database;

        public MngCollectionProviderTest(MngInstanceFixture mngInstanceFixture)
        {
            _mongoClient = mngInstanceFixture.MongoClient;
        }

        public Task InitializeAsync()
        {
            _database = _mongoClient.GetDatabase("Test");

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _mongoClient.DropDatabaseAsync("Test");
        }

        [Fact]
        public void GetCollection_Should_Return_Registered_Collection_Instance()
        {
            //Arrange
            MngCollectionProvider collectionProvider = new MngCollectionProvider(_database);
            collectionProvider.RegisterCollection<TestEntity>("TestEntities");

            //Act
            IMongoCollection<TestEntity> collection = collectionProvider.GetCollection<TestEntity>();

            //Assert
            Assert.NotNull(collection);
        }
    }
}
