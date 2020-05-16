using System.Threading.Tasks;

namespace AppCore.Orm
{
    interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        Task SaveChangesAsync();
    }
}
