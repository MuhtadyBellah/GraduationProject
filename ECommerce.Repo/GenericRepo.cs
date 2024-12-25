using ECommerce.Core.Models;
using ECommerce.Core.Repos;
using ECommerce.Core.Specifications;
using ECommerce.Repo.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repo
{
    public class GenericRepo<T> : IGenericRepo<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;
        public GenericRepo(StoreContext dbContext) => _dbContext = dbContext;

        private IQueryable<T> Spec(ISpecific<T> spec) 
            => SpecificEvalutor<T>.GetQuery(_dbContext.Set<T>(), spec);
        
        public async Task<IEnumerable<T>> GetAllAsync(ISpecific<T> spec)
            => await Spec(spec).ToListAsync();

        public async Task<T> GetByIdAsync(ISpecific<T> spec)
            => await Spec(spec).FirstOrDefaultAsync();

        public async Task<int> GetCountAsync(ISpecific<T> spec) 
            => await Spec(spec).CountAsync();


        #region Without Specification
        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbContext.Set<T>().ToListAsync();

        public async Task<T> GetByIdAsync(int id)
            => await _dbContext.Set<T>().FindAsync(id);
        #endregion

        public async Task UpdateAsync(T updatedEntity)
        {
            _dbContext.Set<T>().Update(updatedEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateAsync(T createdEntity)
        {
            await _dbContext.Set<T>().AddAsync(createdEntity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
