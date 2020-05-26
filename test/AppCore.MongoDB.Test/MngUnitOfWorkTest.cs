using AppCore.MongoDB.Test.Configuration;
using MongoDB.Driver;
using System;
using System.Data;
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
            await _database.CreateCollectionAsync("TestEntities");
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
        public async Task BeginAsync_Should_Throw_Exception_When_Already_Has_Active_Transaction()
        {
            //Arrange
            await _mngUnitOfWork.BeginAsync();
            //Act
            //Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _mngUnitOfWork.BeginAsync());
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

        [Fact]
        public async Task RollbackAsync_Should_Rollback_Uncommitted_Changes_To_Underlying_Database()
        {
            //Arrange
            await _mngUnitOfWork.BeginAsync();
            await _mngRepository.InsertAsync(_entity);

            //Act
            await _mngUnitOfWork.RollbackAsync();

            //Assert
            var filter = Builders<TestEntity>.Filter.Eq("Id", _entity.Id);
            Assert.Null(_collection.Find(filter).SingleOrDefault());
        }

        [Fact]
        public async Task Should_Begin_Another_Transaction_After_Previous_Transaction_Committed()
        {
            //Arrange
            await _mngUnitOfWork.BeginAsync();
            await _mngRepository.InsertAsync(_entity);
            await _mngUnitOfWork.CommitAsync();

            //Act
            await _mngUnitOfWork.BeginAsync();
            TestEntity testEntity2 = new TestEntity { Id = 2, Value = "Beşiktaşş" };
            await _mngRepository.InsertAsync(testEntity2);
            await _mngUnitOfWork.CommitAsync();

            //Assert
            Assert.True(_collection.EstimatedDocumentCount() == 2);
        }

        [Fact]
        public async Task Should_Get_Uncommited_Entity_Count()
        {
            //Arrange
            await _mngUnitOfWork.BeginAsync();
            await _mngRepository.InsertAsync(_entity);
            await _mngRepository.InsertAsync(new TestEntity { Id = 2, Value = "Karakartal" });

            ClientSessionOptions clientSessionOptions = new ClientSessionOptions();
            clientSessionOptions.DefaultTransactionOptions = new TransactionOptions(readConcern:ReadConcern.Local);

            var newSession = await _mongoClient.StartSessionAsync(clientSessionOptions);
            var newCollection = _database.GetCollection<TestEntity>("TestEntities");

            //Act
            var results = newCollection.Find(newSession, Builders<TestEntity>.Filter.Empty);

            //Assert
            Assert.True(results.Count() == 2);
        }
    }
}
