using Microsoft.EntityFrameworkCore;

namespace AppCore.Orm.EntityFramework.Test.Configuration
{
    public class TestDbContext : DbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; }
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>().ToTable("TestEntity");

            modelBuilder.Entity<TestEntity>(
                                               b =>
                                               {
                                                   b.Property("Id");
                                                   b.HasKey("Id");
                                                   b.Property(e => e.Value);
                                               });

            base.OnModelCreating(modelBuilder);
        }
    }
}
