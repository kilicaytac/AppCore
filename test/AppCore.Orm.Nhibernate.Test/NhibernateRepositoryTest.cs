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
        private ISession Session { get { return _sessionProvider.GetSession(); } }
        private TestEntityRepository _nhRepository;
        private TestEntity _entity;
        private INhSessionProvider _sessionProvider;
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

            _sessionProvider = new NhScopedSessionProvider(_sessionFactory, FlushMode.Always);
            _nhRepository = new TestEntityRepository(_sessionProvider);
            _entity = new TestEntity { Id = 1, Value = "Hello World" };

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

            if (Session != null)
            {
                Session.Dispose();
            }

            return Task.CompletedTask;
        }

        [Fact]
        public async Task InsertAsync_Should_Add_Entity_To_Underlying_Database()
        {
            //Arrange

            //Act
            await _nhRepository.InsertAsync(_entity);

            //Assert
            Assert.NotNull(Session.Get<TestEntity>(_entity.Id));
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Entity_In_Underlying_Database()
        {
            //Arrange
            await _nhRepository.InsertAsync(_entity);
            _entity.Value = "Beþiktaþ";

            //Act
            await _nhRepository.UpdateAsync(_entity);

            //Assert
            Assert.Equal(Session.Get<TestEntity>(_entity.Id).Value, _entity.Value);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Existing_Entity_From_Underlying_Database()
        {
            //Arrange
            await _nhRepository.InsertAsync(_entity);

            //Act
            await _nhRepository.DeleteAsync(_entity);

            //Assert
            Assert.Null(Session.Get<TestEntity>(_entity.Id));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Get_Entity_When_Entity_Is_Exist_In_Underlying_Database()
        {
            //Arrange
            await _nhRepository.InsertAsync(_entity);

            //Act
            TestEntity result = await _nhRepository.GetByIdAsync<int>(_entity.Id);

            //Assert
            Assert.NotNull(result);
        }
    }
}
