using AutoMapper;
using ECommerce.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Core.Models;
using ECommerce.Errors;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductType>> GetProductTypes(int id)
            => Ok(await _repos.Repo<ProductType>().GetByIdAsync(id));

        [HttpGet("count")]
        public async Task<ActionResult<int>> GetNumBrands()
            => Ok((await _repos.Repo<ProductType>().GetAllAsync()).Count());

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductType(int id, [FromForm] ProductType productType)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var exist = await _repos.Repo<ProductType>().GetByIdAsync(id);
            if (exist is null) return BadRequest(new ApiResponse(400));

            exist = productType;
            try
            {
                _repos.Repo<ProductType>().Update(exist);
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
        public async Task<ActionResult<ProductType>> PostProductType([FromForm] ProductType productType)
        {
            if (productType.Id <= 0) return BadRequest(new ApiResponse(400));
            var type = new ProductType()
            {
                Name = productType.Name,
                Description = productType.Description
            };
            try
            {
                await _repos.Repo<ProductType>().AddAsync(type);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500, ex.Message));
            }

            return Ok(productType);
        }

        [HttpDelete("{id}")]
        [Authorize("Admin")]
        public async Task<IActionResult> DeleteProductType(int id)
        {
            var productType = await _repos.Repo<ProductType>().GetByIdAsync(id); ;
            if (productType == null) return NotFound(new ApiResponse(404));

            try
            {
                _repos.Repo<ProductType>().Delete(productType);
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
