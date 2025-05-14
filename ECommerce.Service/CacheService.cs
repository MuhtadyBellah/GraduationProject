using ECommerce.Core.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Service
{
    public class CacheService : ICache
    {
        private readonly IDatabase _database;

        public CacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task CacheDataAsync(string key, object value, TimeSpan ExpireTime)
        {
            if(value is null) return;
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var serializedValue = JsonSerializer.Serialize(value, options);
            await _database.StringSetAsync(key, serializedValue, ExpireTime);
        }

        public async Task<string?> GetDataAsync(string key)
        {
            var res = await _database.StringGetAsync(key);
            return (res.IsNullOrEmpty) ? null : res.ToString();
        }
    }
}
