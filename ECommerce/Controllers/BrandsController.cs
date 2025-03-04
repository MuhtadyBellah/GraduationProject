using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.Core.Repos;
using ECommerce.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    public class BrandsController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitWork _repos;
        public BrandsController(IMapper mapper, IUnitWork repos)
        {
            _mapper = mapper;
            _repos = repos;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> GetProductBrands()
            => Ok(await _repos.Repo<ProductBrand>().GetAllAsync());

        /* PutBrand
        [HttpPut("Brands/{id}")]
        public async Task<IActionResult> PutProductBrand(int id, ProductBrand productBrand)
        {
            if (id != productBrand.Id)
            {
                return BadRequest();
            }

            _context.Entry(productBrand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductBrandExists(id))
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
        /* PostBrand
        [HttpPost]
        public async Task<ActionResult<ProductBrand>> PostProductBrand(ProductBrand productBrand)
        {
            _context.ProductBrands.Add(productBrand);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductBrand", new { id = productBrand.Id }, productBrand);
        }
        */
        /* DeleteBrand
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductBrand(int id)
        {
            var productBrand = await _context.ProductBrands.FindAsync(id);
            if (productBrand == null)
            {
                return NotFound();
            }

            _context.ProductBrands.Remove(productBrand);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductBrandExists(int id)
        {
            return _context.ProductBrands.Any(e => e.Id == id);
        }
        */
    }
}
