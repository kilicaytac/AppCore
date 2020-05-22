using Mongo2Go;
using MongoDB.Driver;
using System.Threading.Tasks;
using Xunit;

namespace AppCore.MongoDB.Test.Configuration
{
    public class MngInstanceFixture : IAsyncLifetime
    {
        private MongoDbRunner _runner;
        private MongoClient _mongoClient;

        public MongoClient MongoClient { get { return _mongoClient; } }
        public MngInstanceFixture()
        {

        }
        public Task InitializeAsync()
        {
            _runner = MongoDbRunner.StartForDebugging();
            var url = new MongoUrl(_runner.ConnectionString);
            var mongoClientSettings = MongoClientSettings.FromUrl(url);
            mongoClientSettings.RetryWrites = false;
            _mongoClient = new MongoClient(mongoClientSettings);
            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _runner.Dispose();

            return Task.CompletedTask;
        }
    }

    [CollectionDefinition("MngInstanceCollection", DisableParallelization = true)]
    public class MngInstanceCollection : ICollectionFixture<MngInstanceFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
