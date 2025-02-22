using AutoMapper;
using ECommerce.Core.Models.Basket;
using ECommerce.Core.RepoInterface;
using ECommerce.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    public class BasketsController : ApiBaseController
    {
        private readonly IBasketRepo _basketRepo;
        private readonly IMapper _mapper;

        public BasketsController(IBasketRepo basketRepo, IMapper mapper)
        {
            _basketRepo = basketRepo;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string id)
        {
            var basket = await _basketRepo.GetBasketAsync(id);
            return (basket is null) ? new CustomerBasket(id) : Ok(basket);
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDTO basket)
        {
            var MappedBasket = _mapper.Map<CustomerBasketDTO, CustomerBasket>(basket);
            var updated = await _basketRepo.UpdateBasketAsync(MappedBasket);
            return (basket is null) ? BadRequest(basket) : Ok(basket);
        }

        [HttpDelete("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> DeleteBasket(string basketId)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);
            if (basket == null)
            {
                return NotFound();
            }

            var result = await _basketRepo.DeleteBaketAsync(basketId);
            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting basket");
            }

            return NoContent();
        }
    }
}
