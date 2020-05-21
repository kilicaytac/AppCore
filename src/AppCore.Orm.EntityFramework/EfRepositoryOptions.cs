using Microsoft.EntityFrameworkCore;

namespace AppCore.Orm.EntityFramework
{
    public class EfRepositoryOptions
    {
        public DbContext DbContext { get; set; }
        public bool AutoFlushEnabled { get; set; }
    }
}
