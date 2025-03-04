using AutoMapper;
using ECommerce.Core.Repos;
using ECommerce.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Core.Models;

namespace ECommerce.Controllers
{
    public class CategoriesController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitWork _repos;

        public CategoriesController(IMapper mapper, IUnitWork repos)
        {
            _mapper = mapper;
            _repos = repos;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductTypes()
            => Ok(await _repos.Repo<ProductType>().GetAllAsync());

        /* PutType
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductType(int id, ProductType productType)
        {
            if (id != productType.Id)
            {
                return BadRequest();
            }

            _context.Entry(productType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        */
        /* PostType
        [HttpPost]
        public async Task<ActionResult<ProductType>> PostProductType(ProductType productType)
        {
            _context.ProductTypes.Add(productType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductType", new { id = productType.Id }, productType);
        }
        */
        /* DeleteType
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductType(int id)
        {
            var productType = await _context.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }

            _context.ProductTypes.Remove(productType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

    }
}
