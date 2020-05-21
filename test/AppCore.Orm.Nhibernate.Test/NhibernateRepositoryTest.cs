using System.Data.SQLite;
using System.Threading.Tasks;
using AppCore.Orm.Nhibernate.Test.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using Xunit;
using NhibernateCore = NHibernate;

namespace AppCore.Orm.Nhibernate.Test
{
    public class NhibernateRepositoryTest : IAsyncLifetime
    {
        private SQLiteConnection _sqliteConnection;
        private ISessionFactory _sessionFactory;
        private ISession _session;
        private TestEntityRepository _repository;
        
        public NhibernateRepositoryTest()
        {

        }
        public Task InitializeAsync()
        {
            string ConnectionString = "FullUri=file:memorydb.db?mode=memory&cache=shared";
            var configuration = Fluently.Configure()
                                         .Database(SQLiteConfiguration.Standard.ConnectionString(ConnectionString))
                                         .Mappings(m => m.FluentMappings.Add<TestEntityMap>())
                                         .ExposeConfiguration(x => x.SetProperty("current_session_context_class", "call"))
                                         .BuildConfiguration();

            var schemaExport = new SchemaExport(configuration);
            _sqliteConnection = new SQLiteConnection(ConnectionString);
            _sqliteConnection.Open();
            schemaExport.Execute(false, true, false, _sqliteConnection, null);

            _sessionFactory = configuration.BuildSessionFactory();
            _session = _sessionFactory.OpenSession();

            _repository = new TestEntityRepository(_session);

            return Task.CompletedTask;
        }
        public Task DisposeAsync()
        {
            if (_sqliteConnection != null)
            {
                _sqliteConnection.Close();
                _sqliteConnection.Dispose();
            }

            if (_sessionFactory != null)
            {
                _sessionFactory.Dispose();
            }

            if (_session != null)
            {
                _session.Dispose();
            }

            return Task.CompletedTask;
        }

        [Fact]
        public async Task InsertAsync_Should_Add_Entity_To_Underlying_Database()
        {
            //Arrange
            TestEntity entity = new TestEntity { Id = 1, Value = "Hello World" };

            //Act
            await _repository.InsertAsync(entity);

            //Assert
            Assert.NotNull(_session.Get<TestEntity>(entity.Id));
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Entity_In_Underlying_Database()
        {
            //Arrange
            TestEntity entity = new TestEntity { Id = 1, Value = "Hello World" };
            await _repository.InsertAsync(entity);
            entity.Value = "Beþiktaþ";

            //Act
            await _repository.UpdateAsync(entity);

            //Assert
            Assert.Equal(_session.Get<TestEntity>(entity.Id).Value, entity.Value);
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
            Assert.Null(_session.Get<TestEntity>(entity.Id));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Get_Entity_When_Entity_Is_Exist_In_Underlying_Database()
        {
            //Arrange
            TestEntity entity = new TestEntity { Id = 1, Value = "Hello World" };
            await _repository.InsertAsync(entity);

            //Act
            TestEntity result = await _repository.GetByIdAsync<int>(entity.Id);

            //Assert
            Assert.NotNull(result);
        }
    }
}
