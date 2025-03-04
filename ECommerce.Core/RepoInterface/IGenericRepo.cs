using ECommerce.Core.Models;
using ECommerce.Core.Specifications;

namespace ECommerce.Core.Repos
{
    public interface IGenericRepo<T> where T : BaseEntity
    {
        public Task<IEnumerable<T>> GetAllAsync(ISpecific<T> spec);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T?> GetByIdAsync(ISpecific<T> spec);
        public Task<T?> GetByIdAsync(int id);
        public Task<int> GetCountAsync(ISpecific<T> spec);


        public Task AddAsync(T item);
        public void Update(T item);
        public void Delete(T item);
    }
}
