using ECommerce.Core.Models;
using ECommerce.Core.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core
{
    public interface IUnitWork : IAsyncDisposable
    {
        IGenericRepo<TEntity> Repo<TEntity>() where TEntity : BaseEntity;
        Task<int> CompleteAsync();
    }
}
