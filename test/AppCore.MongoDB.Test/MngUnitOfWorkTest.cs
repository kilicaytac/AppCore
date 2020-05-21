using AppCore.MongoDB.Test.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
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
        private MngRepository<TestEntity> _mngRepository;
        private MngUnitOfWork _mngUnitOfWork;
        private TransactionContext _transactionContext;
        public MngUnitOfWorkTest(MngInstanceFixture mngInstanceFixture)
        {
            _mongoClient = mngInstanceFixture.MongoClient;
        }
        public Task InitializeAsync()
        {
            _database = _mongoClient.GetDatabase("Test");
            _database.CreateCollection("TestEntities");
            _collection = _database.GetCollection<TestEntity>("TestEntities");
            _mngRepository = new MngRepository<TestEntity>(_collection, new TransactionContext());
            _transactionContext = new TransactionContext();
            _mngUnitOfWork = new MngUnitOfWork(_mongoClient, _transactionContext);

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            if (_transactionContext.Session != null)
            {
                _transactionContext.Session.Dispose();
            }

            await _mongoClient.DropDatabaseAsync("Test");
        }

        [Fact]
        public async Task BeginAsync_Should_Open_Transaction_To_Underlying_Database()
        {
            //Arrange

            //Act
            await _mngUnitOfWork.BeginAsync();

            //Assert
            Assert.NotNull(_transactionContext.Session);
            Assert.True(_transactionContext.Session.IsInTransaction);
        }

        [Fact]
        public async Task CommitAsync_Should_Commit_Transaction_Changes_To_Underlying_Database()
        {
            //Arrange
            await _mngUnitOfWork.BeginAsync();
            TestEntity entity = new TestEntity { Id = 1, Value = "Beşiktaş" };
            await _mngRepository.InsertAsync(entity);

            //Act
            await _mngUnitOfWork.CommitAsync();

            //Assert
            var filter = Builders<TestEntity>.Filter.Eq("Id", entity.Id);
            Assert.NotNull(_collection.Find(filter).SingleOrDefault());
        }
    }
}
