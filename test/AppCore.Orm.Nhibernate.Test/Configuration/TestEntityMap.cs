using FluentNHibernate.Mapping;

namespace AppCore.Orm.Nhibernate.Test.Configuration
{
    public class TestEntityMap : ClassMap<TestEntity>
    {
        public TestEntityMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned();
            Map(x => x.Value);
            Table("TestEntities");
        }
    }
}
