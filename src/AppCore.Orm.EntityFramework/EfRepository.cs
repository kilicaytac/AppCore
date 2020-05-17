using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AppCore.Orm.EntityFramework
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;

        public EfRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync<IdType>(IdType id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
    }
}
