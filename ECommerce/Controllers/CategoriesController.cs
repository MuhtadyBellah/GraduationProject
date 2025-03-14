using AutoMapper;
using ECommerce.Core.Repos;
using ECommerce.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Core.Models;
using ECommerce.Errors;
using NuGet.Protocol.Plugins;

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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductType(int id, ProductType productType)
        {
            if (id != productType.Id || id <= 0) return BadRequest(new ApiResponse(400));

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
        public async Task<ActionResult<ProductType>> PostProductType(ProductType productType)
        {
            if (productType.Id <= 0) return BadRequest(new ApiResponse(400));
            try
            {
                await _repos.Repo<ProductType>().AddAsync(productType);
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
