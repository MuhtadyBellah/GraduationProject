using ECommerce.Core.Models.Basket;
using ECommerce.Core.RepoInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    public class BasketsController : ApiBaseController
    {
        private readonly IBasketRepo _basketRepo;

        public BasketsController(IBasketRepo basketRepo)
        {
            _basketRepo = basketRepo;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string id)
        {
            var basket = await _basketRepo.GetBaketAsync(id);
            return (basket is null) ? new CustomerBasket(id) : Ok(basket);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasket basket)
        {
            var updated = await _basketRepo.UpdateBaketAsync(basket);
            return (basket is null) ? BadRequest(basket) : Ok(basket);
        }

        [HttpDelete]
        public async Task<ActionResult<CustomerBasket>> DeleteBasket(string basketId)
            => await _basketRepo.GetBaketAsync(basketId);
    }
}
