using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppCore.Orm.Nhibernate
{
    public interface INhSessionProvider
    {
        ISession GetSession();
    }
}
