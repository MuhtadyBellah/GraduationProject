using ECommerce.Core.Models.Basket;
using ECommerce.Core.RepoInterface;
using StackExchange.Redis;
using System.Text.Json;

namespace ECommerce.Repo
{
    public class BasketRepo : IBasketRepo
    {
        private readonly IDatabase _database;
        public BasketRepo(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task<bool> DeleteBaketAsync(string basketId)
            => await _database.KeyDeleteAsync(basketId);

        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var basket = await _database.StringGetAsync(basketId);
            return (basket.IsNull) ? null : JsonSerializer.Deserialize<CustomerBasket>(basket.ToString());
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var jsonBasket = JsonSerializer.Serialize(basket);
            var updated = await _database.StringSetAsync(basket.Id, jsonBasket, TimeSpan.FromDays(1));
            return  ((!updated)? null : await GetBasketAsync(basket.Id));
        }
    }
}
