using AppCore.Orm.Nhibernate.Test.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Data.SQLite;
using System.Threading.Tasks;
using Xunit;

namespace AppCore.Orm.Nhibernate.Test
{
    public class NhUnitOfWorkTest : IAsyncLifetime
    {
        private SQLiteConnection _sqliteConnection;
        private ISessionFactory _sessionFactory;
        private ISession Session { get { return _sessionProvider.GetSession(); } }
        private NhRepository<TestEntity> _nhRepository;
        private NhUnitOfWork _nhUnitofWork;
        private TestEntity _entity;
        private INhSessionProvider _sessionProvider;
        public NhUnitOfWorkTest()
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

            _nhRepository = new NhRepository<TestEntity>(_sessionProvider);
            _nhUnitofWork = new NhUnitOfWork(_sessionProvider);
            _entity = new TestEntity { Id = 1, Value = "Beşiktaş" };

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
        public async Task BeginAsync_Should_Open_Transaction_To_Underlying_Database()
        {
            //Arrange

            //Act
            await _nhUnitofWork.BeginAsync();

            //Assert
            Assert.NotNull(Session.Transaction);
        }

        [Fact]
        public async Task BeginAsync_Should_Throw_Exception_When_Already_Has_Active_Transaction()
        {
            //Arrange
            await _nhUnitofWork.BeginAsync();
            //Act
            //Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _nhUnitofWork.BeginAsync());
        }

        [Fact]
        public async Task CommitAsync_Should_Commit_Transaction_Changes_To_Underlying_Database()
        {
            //Arrange
            await _nhUnitofWork.BeginAsync();
            await _nhRepository.InsertAsync(_entity);

            //Act
            await _nhUnitofWork.CommitAsync();

            //Assert
            Assert.NotNull(Session.Get<TestEntity>(_entity.Id));
        }

        [Fact]
        public async Task RollbackAsync_Should_Rollback_Uncommitted_Changes_To_Underlying_Database()
        {
            //Arrange
            await _nhUnitofWork.BeginAsync();
            await _nhRepository.InsertAsync(_entity);

            //Act
            await _nhUnitofWork.RollbackAsync();

            //Assert
            Assert.NotNull(Session.Get<TestEntity>(_entity.Id));
        }

        [Fact]
        public async Task Should_Begin_Another_Transaction_After_Previous_Transaction_Committed()
        {
            //Arrange
            await _nhUnitofWork.BeginAsync();
            await _nhRepository.InsertAsync(_entity);
            await _nhUnitofWork.CommitAsync();

            //Act
            await _nhUnitofWork.BeginAsync();
            TestEntity testEntity2 = new TestEntity { Id = 2, Value = "Beşiktaşş" };
            await _nhRepository.InsertAsync(testEntity2);
            await _nhUnitofWork.CommitAsync();

            //Assert
            Assert.True(Session.QueryOver<TestEntity>().RowCount() == 2);
        }

        [Fact]
        public async Task Should_Get_Uncommited_Entity_When_BeginAsync_Called_With_ReadUncommitted_Isolation_Level()
        {
            //Arrange
            await _nhUnitofWork.BeginAsync(System.Data.IsolationLevel.Unspecified);
            await _nhRepository.InsertAsync(_entity);

            //Act
            TestEntity result = await Session.GetAsync<TestEntity>(_entity.Id);

            //Assert
            Assert.NotNull(result);
        }
    }
}
