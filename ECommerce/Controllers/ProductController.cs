using AutoMapper;
using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Repos;
using ECommerce.Core.Specifications;
using ECommerce.DTO;
using ECommerce.Errors;
using ECommerce.Helper;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    public class ProductController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitWork _repos;

        public ProductController(IMapper mapper, IUnitWork repos)
        {
            _mapper = mapper;
            _repos = repos;
        }

        [HttpGet]
        [CachedAttribute(300)]
        [ProducesResponseType(typeof(Pagination<ProductDTO>), 200)]
        public async Task<ActionResult<Pagination<ProductDTO>>> GetProducts([FromQuery] ProductSpecParams param)
        {
            var spec = new ProductSpecific(param);
            var products = await _repos.Repo<Product>().GetAllAsync(spec);
            var mapProducts = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(products);
            var CountSpec = new ProductFilterationCount(param);
            var count = await _repos.Repo<Product>().GetCountAsync(CountSpec);
            var pagination = new Pagination<ProductDTO>(param.PageIndex, param.PageSize, mapProducts, count);
            return Ok(pagination);
        }

        private async Task<ProductDTO?> GetProductDTO(ProductSpecific spec)
        {
            var product = await _repos.Repo<Product>().GetByIdAsync(spec);
            return product is null ? null : _mapper.Map<Product, ProductDTO>(product);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var spec = new ProductSpecific(id);
            var MapProduct = await GetProductDTO(spec);
            return MapProduct is null ? NotFound(new ApiResponse(404)) : Ok(MapProduct);
        }

        [HttpGet("{id}/img")]
        [ProducesResponseType(typeof(HttpContent), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult> GetItemPictureById(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var spec = new ProductSpecific(id);
            var MapProduct = await GetProductDTO(spec);
            return MapProduct is null ? NotFound(new ApiResponse(404)) : Redirect(MapProduct.PictureUrl);
        }

        [HttpGet("{id}/imgGlb")]
        [ProducesResponseType(typeof(HttpContent), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult> GetItemPictureGlb(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var spec = new ProductSpecific(id);
            var MapProduct = await GetProductDTO(spec);
            return MapProduct is null ? NotFound(new ApiResponse(404)) : Ok(MapProduct.UrlGlb);
        }
        
        [HttpPut]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<ProductDTO>> PutProduct(ProductDTO updated)
        {
            if (updated is null) return BadRequest(new ApiResponse(400));

            var spec = new ProductSpecific(updated.Id);
            var product = await _repos.Repo<Product>().GetByIdAsync(spec);
            if (product is null)
                return NotFound(new ApiResponse(404));
            
            var productBrand = await _repos.Repo<ProductBrand>().GetByIdAsync(updated.ProductBrandId);
            if (productBrand is null)
                return BadRequest(new ApiResponse(400, "Invalid ProductBrandId"));

            var productType = await _repos.Repo<ProductType>().GetByIdAsync(updated.ProductTypeId);
            if (productType is null)
                return BadRequest(new ApiResponse(400, "Invalid ProductTypeId"));

            product.Name = updated.Name ?? product.Name;
            product.Description = updated.Description ?? product.Description;
            product.Price = updated.Price;
            product.Quantity = updated.Quantity;
            product.ProductBrandId = updated.ProductBrandId;
            product.ProductTypeId = updated.ProductTypeId;
            product.PictureUrl = updated.PictureUrl ?? product.PictureUrl;
            product.UrlGlb = updated.UrlGlb ?? product.UrlGlb;

            try
            {
                _repos.Repo<Product>().Update(product);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500, $"An error occurred while saving the product: {ex.Message}, StackTrace: {ex.StackTrace}"));
            }
            var mapProductDTO = _mapper.Map<Product, ProductDTO>(product);
            return Ok(mapProductDTO);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<ProductDTO>> PostProduct(ProductDTO created)
        {
            if (created is null) return BadRequest(new ApiResponse(400));

            var productBrand = await _repos.Repo<ProductBrand>().GetByIdAsync(created.ProductBrandId);
            if (productBrand is null)
                return BadRequest(new ApiResponse(400, "Invalid ProductBrandId"));

            var productType = await _repos.Repo<ProductType>().GetByIdAsync(created.ProductTypeId);
            if (productType is null)
                return BadRequest(new ApiResponse(400, "Invalid ProductTypeId"));

            var file = Path.GetFileName(created.PictureUrl);
            if (Path.GetExtension(file).ToLower() == ".jpg" ||
                Path.GetExtension(file).ToLower() == ".jpeg" ||
                Path.GetExtension(file).ToLower() == ".png")
            { }
            else return BadRequest("Invalid image format. Only JPG, JPEG and PNG are allowed.");

            var glb = Path.GetFileName(created.UrlGlb);
            if (Path.GetExtension(glb).ToLower() != ".glp")
                return BadRequest("Invalid image format. Only GLB  are allowed.");

            var res = "Images/Products/";
            var product = new Product()
            {
                Name = created.Name,
                Description = created.Description,
                Price = created.Price,
                ProductBrandId = created.ProductBrandId,
                ProductTypeId = created.ProductTypeId,
                Quantity = created.Quantity,
                PictureUrl = file,
                UrlGlb = glb
            };
            
            await _repos.Repo<Product>().AddAsync(product);
            await _repos.CompleteAsync();
            
            var mapProductDTO = _mapper.Map<Product, ProductDTO>(product);
            return Ok(mapProductDTO);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var spec = new ProductSpecific(id);
            var product = await _repos.Repo<Product>().GetByIdAsync(spec);
            if (product is null)
                return NotFound(new ApiResponse(404));

            try
            { 
                _repos.Repo<Product>().Delete(product);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500, $"An error occurred while saving the product: {ex.Message}, StackTrace: {ex.StackTrace}"));
            }
            return Ok(new ApiResponse(200, "Product deleted successfully"));
        }
    }
}
