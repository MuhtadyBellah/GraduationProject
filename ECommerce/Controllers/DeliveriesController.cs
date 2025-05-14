using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Core.Models.Order;
using ECommerce.Repo.Data;
using ECommerce.Core;
using ECommerce.Errors;
using AutoMapper;
using ECommerce.DTO;
using ECommerce.Core.Specifications;

using ECommerce.Helper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    public class DeliveriesController : ApiBaseController
    {
        private readonly IUnitWork _repos;
        private readonly IMapper _mapper;

        public DeliveriesController(IUnitWork repos, IMapper mapper)
        {
            _repos = repos;
            this._mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DeliveryDTO>), 200)]
        public async Task<ActionResult<IEnumerable<DeliveryDTO>>> GetDeliveries()
        {
            var deliveries = await _repos.Repo<Delivery>().GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<DeliveryDTO>>(deliveries));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DeliveryDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<DeliveryDTO>> GetDelivery(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var spec = new deliverSpec(id);
            var delivery = await _repos.Repo<Delivery>().GetByIdAsync(spec);
            if (delivery is null)
                return NotFound(new ApiResponse(404));

            var mapped = _mapper.Map<DeliveryDTO>(delivery);
            return Ok(mapped);
        }

        // POST: api/deliveries
        [HttpPost]
        [ProducesResponseType(typeof(DeliveryDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<DeliveryDTO>> CreateDelivery([FromBody] Delivery delivery)
        {
            var newDelivery = new Delivery()
            {
                SName = delivery.SName,
                DeliveryTime = delivery.DeliveryTime,
                Description = delivery.Description,
                Status = DeliveryStatus.Pending,
                lateLatiude = delivery.lateLatiude,
                longLatiude = delivery.longLatiude,
                Cost = 0
            };

            try
            {
                await _repos.Repo<Delivery>().AddAsync(newDelivery);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500));
            }

            var mapped = _mapper.Map<DeliveryDTO>(newDelivery);
            return Ok(mapped);
        }

        // PUT: api/deliveries/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DeliveryDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<DeliveryDTO>> UpdateDelivery(int id, [FromBody] Delivery updatedDelivery)
        {
            if (id != updatedDelivery.Id)
                return BadRequest("ID mismatch");

            var spec = new deliverSpec(id);
            var existing = await _repos.Repo<Delivery>().GetByIdAsync(spec);
            if (existing == null)
                return NotFound(new ApiResponse(404));

            existing.SName = updatedDelivery.SName;
            existing.Description = updatedDelivery.Description;
            existing.DeliveryTime = updatedDelivery.DeliveryTime;
            existing.Status = updatedDelivery.Status;
            existing.longLatiude = updatedDelivery.longLatiude;
            existing.lateLatiude = updatedDelivery.lateLatiude;

            try
            {
                _repos.Repo<Delivery>().Update(existing);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500));
            }

            var mapped = _mapper.Map<DeliveryDTO>(existing);
            return Ok(mapped);
        }

        [HttpPut("{id}/costs")]
        [Authorize("Sanctum")]
        [ProducesResponseType(typeof(DeliveryDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<DeliveryDTO>> UpdateDeliveryCost(int id, decimal userLongitude, decimal userLatitude)
        {
            var spec = new deliverSpec(id);
            var delivery = await _repos.Repo<Delivery>().GetByIdAsync(spec);
            if (delivery == null)
                return NotFound(new ApiResponse(404));

            var distance = DeliveryCostCalculator.CalculateDistanceInKm(
                userLatitude, userLongitude,
                delivery.lateLatiude, delivery.longLatiude
            );
            var cost = DeliveryCostCalculator.CalculateCost(distance);

            delivery.Cost = cost;
            try
            {
                _repos.Repo<Delivery>().Update(delivery);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500));
            }
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userOrders = delivery.Orders.Where(o => o.UserId == int.Parse(userId)).ToList();
            var ordersTotal = userOrders.Sum(o => o.Total);
            var grandTotal = ordersTotal + cost;

            var mapped = _mapper.Map<DeliveryDTO>(delivery);
            mapped = mapped with { DistanceInKm = Math.Round(distance, 2), userOrdersTotal = ordersTotal, totalSales = grandTotal };
            return Ok(mapped);
        }
    }
}
