using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Repos;
using ECommerce.Repo.Data;
using System.Collections;

namespace ECommerce.Repo
{
    public class UnitWork : IUnitWork
    {
        private readonly StoreContext _dbContext;
        private Hashtable _repos;

        public UnitWork(StoreContext dbcontext)
        {
            _repos = new Hashtable();
            _dbContext = dbcontext;
        }

        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();

        public ValueTask DisposeAsync()
            => _dbContext.DisposeAsync();

        public IGenericRepo<TEntity> Repo<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity).Name;
            if (!_repos.ContainsKey(type))
            {
                var repo = new GenericRepo<TEntity>(_dbContext);
                _repos.Add(type, repo);
            }
            return _repos[type] as IGenericRepo<TEntity>;
        }
    }
}
