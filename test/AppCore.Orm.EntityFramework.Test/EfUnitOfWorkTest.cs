﻿using AppCore.Orm.EntityFramework.Test.Configuration;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace AppCore.Orm.EntityFramework.Test
{
    public class EfUnitOfWorkTest : IAsyncLifetime
    {
        private SqliteConnection _sqliteConnection;
        private DbContextOptions<TestDbContext> _testDbContextDbOptions;

        private TestDbContext _testDbContext;
        private EfUnitOfWork _efUnitofWork;
        private EfRepository<TestEntity> _repository;
        public EfUnitOfWorkTest()
        {

        }

        public async Task InitializeAsync()
        {
            _sqliteConnection = new SqliteConnection("DataSource=:memory:");

            _sqliteConnection.Open();

            _testDbContextDbOptions = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(_sqliteConnection)
                .Options;


            using (TestDbContext testDbContext = new TestDbContext(_testDbContextDbOptions))
            {
                await testDbContext.Database.EnsureCreatedAsync();
            }

            _testDbContext = new TestDbContext(_testDbContextDbOptions);
            _efUnitofWork = new EfUnitOfWork(_testDbContext);
            _repository = new EfRepository<TestEntity>(_testDbContext);
        }

        public async Task DisposeAsync()
        {
            await _sqliteConnection.DisposeAsync();
        }

        [Fact]
        public async Task BeginAsync_Should_Work_As_Expected()
        {
            //Arrange

            //Act
            await _efUnitofWork.BeginAsync();

            //Assert
            Assert.NotNull(_testDbContext.Database.CurrentTransaction);
        }

        [Fact]
        public async Task BeginAsync_Should_Throw_Exception_When_Already_Has_Active_Transaction()
        {
            //Arrange
            await _efUnitofWork.BeginAsync();
            //Act
            //Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _efUnitofWork.BeginAsync());
        }

        [Fact]
        public async Task CommitAsync_Should_Work_As_Expected()
        {
            //Arrange
            await _efUnitofWork.BeginAsync();
            TestEntity testEntity = new TestEntity { Id = 1, Value = "Beşiktaş" };
            _repository.Add(testEntity);
            await _repository.SaveChangesAsync();

            //Act
            await _efUnitofWork.CommitAsync();

            //Assert
            using (TestDbContext testDbContext = new TestDbContext(_testDbContextDbOptions))
            {
                Assert.NotNull(testDbContext.TestEntities.FirstOrDefault(q => q.Id == testEntity.Id));
            }
        }

        [Fact]
        public async Task RollbackAsync_Should_Work_As_Expected()
        {
            //Arrange
            await _efUnitofWork.BeginAsync();
            TestEntity testEntity = new TestEntity { Id = 1, Value = "Beşiktaş" };
            _repository.Add(testEntity);
            await _repository.SaveChangesAsync();

            //Act
            await _efUnitofWork.RollbackAsync();

            //Assert
            using (TestDbContext testDbContext = new TestDbContext(_testDbContextDbOptions))
            {
                Assert.Null(testDbContext.TestEntities.FirstOrDefault(q => q.Id == testEntity.Id));
            }
        }
    }
}