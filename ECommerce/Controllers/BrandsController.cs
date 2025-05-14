using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Errors;
using ECommerce.Repo.GraphQL.Types;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductBrand>> GetProductBrands(int id)
            => Ok(await _repos.Repo<ProductBrand>().GetByIdAsync(id));

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetNumBrands()
            => Ok((await _repos.Repo<ProductBrand>().GetAllAsync()).Count());


        [HttpPut("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> PutProductBrand(int id, [FromForm] ProductBrand productBrand)
        {
            if (id != productBrand.Id || id <= 0) return BadRequest(new ApiResponse(400));

            var exist = await _repos.Repo<ProductBrand>().GetByIdAsync(id);
            if (exist is null) return BadRequest(new ApiResponse(400));

            exist = productBrand;
            try
            {
                _repos.Repo<ProductBrand>().Update(exist);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500, ex.Message));
            }

            return Ok(exist);
        }

        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<ProductBrand>> PostProductBrand([FromForm] ProductBrand productBrand)
        {
            var brand = new ProductBrand()
            {
                Name = productBrand.Name,
                Description = productBrand.Description
            };
            try
            {
                await _repos.Repo<ProductBrand>().AddAsync(brand);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500, ex.Message));
            }

            return Ok(productBrand);
        }

        [HttpDelete("{id}")]
        [Authorize("Admin")]
        public async Task<ActionResult> DeleteProductBrand(int id)
        {
            var Brand = await _repos.Repo<ProductBrand>().GetByIdAsync(id); ;
            if (Brand == null) return NotFound(new ApiResponse(404));

            try
            {
                _repos.Repo<ProductBrand>().Delete(Brand);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500, ex.Message));
            }

            return Ok(new ApiResponse(200, "Success"));
        }

    }
}
