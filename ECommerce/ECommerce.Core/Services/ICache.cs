using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services
{
    public interface ICache
    {
        Task CacheDataAsync(string key, object value, TimeSpan ExpireTime);
        Task<string?> GetDataAsync(string key);
    }
}
