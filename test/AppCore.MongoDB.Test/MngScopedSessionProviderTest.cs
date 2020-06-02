using AppCore.MongoDB.Test.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;
using Xunit;

namespace AppCore.MongoDB.Test
{
    [Collection("MngInstanceCollection")]
    public class MngScopedSessionProviderTest : IAsyncLifetime
    {
        private readonly MongoClient _mongoClient;
        private IMongoDatabase _database;

        public MngScopedSessionProviderTest(MngInstanceFixture mngInstanceFixture)
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
        public void GetSession_Should_Return_Opened_Session()
        {
            //Arrange
            MngScopedSessionProvider scopedSessionProvider = new MngScopedSessionProvider(_mongoClient);

            //Act
            IClientSessionHandle session = scopedSessionProvider.GetSession();

            //Assert
            Assert.NotNull(session);
        }

        [Fact]
        public void GetSession_Should_Return_Existing_Session_If_Session_Opened()
        {
            //Arrange
            MngScopedSessionProvider scopedSessionProvider = new MngScopedSessionProvider(_mongoClient);

            //Act
            IClientSessionHandle session1 = scopedSessionProvider.GetSession();
            IClientSessionHandle session2 = scopedSessionProvider.GetSession();

            //Assert
            Assert.Equal(session1.ServerSession.Id, session2.ServerSession.Id);
        }
    }
}
