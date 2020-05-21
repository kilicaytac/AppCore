using Microsoft.EntityFrameworkCore;

namespace AppCore.Orm.EntityFramework
{
    public class EfRepositoryOptionsBuilder
    {
        private readonly EfRepositoryOptions _options = null;

        public EfRepositoryOptions Options { get { return _options; } }
        public EfRepositoryOptionsBuilder()
        {
            _options = new EfRepositoryOptions();
        }

        public void SetContext(DbContext dbContext)
        {
            _options.DbContext = dbContext;
        }

        public void EnableAutoFlush()
        {
            _options.AutoFlushEnabled = true;
        }
    }
}
