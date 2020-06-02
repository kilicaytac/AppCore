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
    public class NhScopedSessionProviderTest :IAsyncLifetime
    {
        private SQLiteConnection _sqliteConnection;
        private ISessionFactory _sessionFactory;
       
        public NhScopedSessionProviderTest()
        {

        }

        public Task InitializeAsync()
        {
            string ConnectionString = "FullUri=file:memorydb.db?mode=memory&cache=shared";
            var configuration = Fluently.Configure()
                                         .Database(SQLiteConfiguration.Standard.ConnectionString(ConnectionString))
                                         .Mappings(m => m.FluentMappings.Add<TestEntityMap>())
                                         .ExposeConfiguration(x => x.SetProperty("current_session_context_class", "call").SetProperty("generate_statistics", "true"))
                                         .BuildConfiguration();
            

            var schemaExport = new SchemaExport(configuration);
            _sqliteConnection = new SQLiteConnection(ConnectionString);
            _sqliteConnection.Open();
            schemaExport.Execute(false, true, false, _sqliteConnection, null);

            _sessionFactory = configuration.BuildSessionFactory();

         

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

            return Task.CompletedTask;
        }

        [Fact]
        public void GetSession_Should_Return_Opened_Session()
        {
            //Arrange
            NhScopedSessionProvider scopedSessionProvider = new NhScopedSessionProvider(_sessionFactory);

            //Act
            ISession session = scopedSessionProvider.GetSession();

            //Assert
            Assert.NotNull(session);
            Assert.True(session.IsOpen);
        }

        [Fact]
        public void GetSession_Should_Return_Existing_Session_If_Session_Opened()
        {
            //Arrange
            NhScopedSessionProvider scopedSessionProvider = new NhScopedSessionProvider(_sessionFactory);

            //Act
            ISession session1 = scopedSessionProvider.GetSession();
            ISession session2 = scopedSessionProvider.GetSession();

            //Assert
            Assert.Equal(1, _sessionFactory.Statistics.SessionOpenCount);
        }
    }
}
