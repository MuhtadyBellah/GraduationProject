using ECommerce.Core.Models;
using ECommerce.Core.Specifications;

namespace ECommerce.Core.Repos
{
    public interface IGenericRepo<T> where T : BaseEntity
    {
        Task CreateAsync(T createdEntity);
        public Task<IEnumerable<T>> GetAllAsync(ISpecific<T> spec);
        Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetByIdAsync(ISpecific<T> spec);
        Task<T> GetByIdAsync(int id);
        Task<int> GetCountAsync(ISpecific<T> spec);
        Task UpdateAsync(T updatedEntity);
    }
}
