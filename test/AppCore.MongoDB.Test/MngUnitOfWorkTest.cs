using AppCore.MongoDB.Test.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;
using Xunit;

namespace AppCore.MongoDB.Test
{
    [Collection("MngInstanceCollection")]
    public class MngUnitOfWorkTest : IAsyncLifetime
    {
        private readonly MongoClient _mongoClient;
        private IMongoDatabase _database;
        private IMongoCollection<TestEntity> _collection;
        private IClientSessionHandle _session;
        private MngRepository<TestEntity> _mngRepository;
        private MngUnitOfWork _mngUnitOfWork;
        private TestEntity _entity;

        public MngUnitOfWorkTest(MngInstanceFixture mngInstanceFixture)
        {
            _mongoClient = mngInstanceFixture.MongoClient;
        }
        public async Task InitializeAsync()
        {
            _database = _mongoClient.GetDatabase("Test");
            _collection = _database.GetCollection<TestEntity>("TestEntities");
            _session = await _mongoClient.StartSessionAsync();
            _mngRepository = new MngRepository<TestEntity>(_collection, _session);
            _mngUnitOfWork = new MngUnitOfWork(_session);
            _entity = new TestEntity { Id = 1, Value = "Beşiktaş" };
        }

        public async Task DisposeAsync()
        {
            _session?.Dispose();
            await _mongoClient.DropDatabaseAsync("Test");
        }

        [Fact]
        public async Task BeginAsync_Should_Open_Transaction_To_Underlying_Database()
        {
            //Arrange

            //Act
            await _mngUnitOfWork.BeginAsync();

            //Assert
            Assert.True(_session.IsInTransaction);
        }

        [Fact]
        public async Task CommitAsync_Should_Commit_Transaction_Changes_To_Underlying_Database()
        {
            //Arrange
            await _mngUnitOfWork.BeginAsync();
            await _mngRepository.InsertAsync(_entity);

            //Act
            await _mngUnitOfWork.CommitAsync();

            //Assert
            var filter = Builders<TestEntity>.Filter.Eq("Id", _entity.Id);
            Assert.NotNull(_collection.Find(filter).SingleOrDefault());
        }
    }
}
