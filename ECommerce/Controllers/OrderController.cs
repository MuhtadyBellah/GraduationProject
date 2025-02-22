using AutoMapper;
using ECommerce.Core;
using ECommerce.Core.Models.Order;
using ECommerce.Core.Services;
using ECommerce.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    public class OrderController : ApiBaseController
    {
        private readonly IOrder _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitWork _repos;

        public OrderController(IOrder orderService, IMapper mapper, IUnitWork repos)
        {
            _orderService = orderService;
            this._mapper = mapper;
            this._repos = repos;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderReturnedDTO>> CreateOrderAsync(OrderDTO order)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("User email not found");
            }

            var MappedAddress = _mapper.Map<Address>(order.ShippingAddress);
            var res = await _orderService.CreateOrderAsync(email, order.BasketId, order.DeliveryMethodId, MappedAddress);
            if (res is null) return BadRequest("Problem creating order");

            var MappedOrder = _mapper.Map<Order, OrderReturnedDTO>(res);
            return Ok(MappedOrder);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReturnedDTO>>> GetAllOrdersAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("User email not found");
            }

            var res = await _orderService.GetAllOrdersAsync(email);
            if (res is null) return NotFound("There is no Orders for This User");

            var MappedOrder = _mapper.Map<IEnumerable<Order>, IEnumerable<OrderReturnedDTO>>(res);
            return Ok(MappedOrder);
        }

        [Authorize]
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderReturnedDTO>> GetOrderByIdforUserAsync(int orderId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("User email not found");
            }

            var res = await _orderService.GetOrderByIdforUserAsync(email, orderId);
            if (res is null) return NotFound($"There is no Order with {orderId} for This User");

            var MappedOrder = _mapper.Map<Order, OrderReturnedDTO>(res);
            return Ok(MappedOrder);
        }

        //[Authorize]
        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<IEnumerable<Delivery>>> GetDeliveryMethods()
        {
            var res = await _repos.Repo<Delivery>().GetAllAsync();
            return Ok(res);
        }
    }
}
