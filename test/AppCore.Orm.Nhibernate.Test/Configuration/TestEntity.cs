using System;
using System.Collections.Generic;
using System.Text;

namespace AppCore.Orm.Nhibernate.Test.Configuration
{
    public class TestEntity
    {
        public virtual int Id { get; set; }
        public virtual string Value { get; set; }
    }
}
