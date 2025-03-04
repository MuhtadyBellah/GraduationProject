using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Core.Models.Order;
using ECommerce.Repo.Data;
using ECommerce.Core;
using ECommerce.Errors;
using AutoMapper;

namespace ECommerce.Controllers
{
    public class DeliveriesController : ApiBaseController
    {
        private readonly IUnitWork _repos;

        public DeliveriesController(IUnitWork context)
        {
            _repos = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Delivery>), 200)]
        public async Task<ActionResult<IEnumerable<Delivery>>> GetDeliveries()
            => Ok(await _repos.Repo<Delivery>().GetAllAsync());
    
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Delivery), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<Delivery>> GetDelivery(int id)
        {
            var delivery = await _repos.Repo<Delivery>().GetByIdAsync(id);
            return (delivery == null) ? NotFound(new ApiResponse(404)) : Ok(delivery);
        }

        //// PUT: api/Deliveries/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutDelivery(int id, Delivery delivery)
        //{
        //    if (id != delivery.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _repos.Entry(delivery).State = EntityState.Modified;

        //    try
        //    {
        //        await _repos.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!DeliveryExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Deliveries
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Delivery>> PostDelivery(Delivery delivery)
        //{
        //    _repos.Deliveries.Add(delivery);
        //    await _repos.SaveChangesAsync();

        //    return CreatedAtAction("GetDelivery", new { id = delivery.Id }, delivery);
        //}

        //// DELETE: api/Deliveries/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteDelivery(int id)
        //{
        //    var delivery = await _repos.Deliveries.FindAsync(id);
        //    if (delivery == null)
        //    {
        //        return NotFound();
        //    }

        //    _repos.Deliveries.Remove(delivery);
        //    await _repos.SaveChangesAsync();

        //    return NoContent();
        //}
    }
}
