using AutoMapper;
using ECommerce.ChatServices;
using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Specifications;
using ECommerce.DTO.Request;
using ECommerce.DTO.Response;
using ECommerce.Errors;
using ECommerce.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using System.IO;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    public class ProductController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitWork _repos;
        private readonly Supabase.Client _supabaseClient;

        public ProductController(IMapper mapper, IUnitWork repos, Supabase.Client supabaseClient)
        {
            _mapper = mapper;
            _repos = repos;
            _supabaseClient = supabaseClient;
        }

        [HttpGet]
        [Cached(300)]
        [Authorize("Sanctum")]
        [ProducesResponseType(typeof(Pagination<ProductResponse>), 200)]
        public async Task<ActionResult<Pagination<ProductResponse>>> GetProducts([FromQuery] ProductSpecParams param)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized(new ApiResponse(401));

            var spec = new ProductSpecific(param);
            var products = await _repos.Repo<Product>().GetAllAsync(spec);

            var mapProducts = _mapper.Map<IEnumerable<ProductResponse>>(products, opt => {
                opt.Items["UserId"] = userId;
            });
            var CountSpec = new ProductFilterationCount(param);
            var count = await _repos.Repo<Product>().GetCountAsync(CountSpec);
            var pagination = new Pagination<ProductResponse>(param.PageIndex, param.PageSize, mapProducts, count);
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [Authorize("Sanctum")]
        [ProducesResponseType(typeof(ProductResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ProductResponse>> GetProductById(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId == null) return Unauthorized(new ApiResponse(401));

            var spec = new ProductSpecific(id);
            var product = await _repos.Repo<Product>().GetByIdAsync(spec);

            var MapProduct = product is null ? null : _mapper.Map<ProductResponse>(product, opt => {
                opt.Items["UserId"] = userId;
            });

            return MapProduct is null ? NotFound(new ApiResponse(404)) : Ok(MapProduct);
        }

        [HttpGet("{id}/img")]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult> GetItemPictureById(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var spec = new ProductSpecific(id);
            var product = await _repos.Repo<Product>().GetByIdAsync(spec);
            var mapProducts = _mapper.Map<ProductResponse>(product, opt => {
                opt.Items["UserId"] = null;
            });
            return mapProducts is null ? NotFound(new ApiResponse(404)) : Redirect(mapProducts.PictureUrl);
        }

        [HttpGet("{id}/imgGlb")]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult> GetItemPictureGlb(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var spec = new ProductSpecific(id);
            var product = await _repos.Repo<Product>().GetByIdAsync(spec);
            var mapProducts = _mapper.Map<ProductResponse>(product, opt => {
                opt.Items["UserId"] = null;
            });
            return mapProducts is null ? NotFound(new ApiResponse(404)) : Ok(mapProducts.UrlGlb);
        }
        
        [HttpPut("{id}")]
        [Authorize("Admin")]
        [ProducesResponseType(typeof(ProductResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<ProductResponse>> PutProduct(int id, [FromForm] ProductRequest updated)
        {
            if (updated is null) return BadRequest(new ApiResponse(400));

            var spec = new ProductSpecific(id);
            var product = await _repos.Repo<Product>().GetByIdAsync(spec);
            if (product is null)
                return NotFound(new ApiResponse(404));
            
            var productBrand = await _repos.Repo<ProductBrand>().GetByIdAsync(updated.ProductBrandId);
            if (productBrand is null)
                return BadRequest(new ApiResponse(400, "Invalid ProductBrandId"));

            var productType = await _repos.Repo<ProductType>().GetByIdAsync(updated.ProductTypeId);
            if (productType is null)
                return BadRequest(new ApiResponse(400, "Invalid ProductTypeId"));

            product.Description = updated.Description ?? product.Description;
            product.Price = updated.Price;
            product.Quantity = updated.Quantity;

            try
            {
                if (updated.PictureFile != null && updated.PictureFileGlB != null)
                {
                    using var ms = new MemoryStream();
                    var bucket = _supabaseClient.Storage.From("Images");
                    var extension = Path.GetExtension(updated.PictureFile.FileName).ToLower();
                    if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                        return BadRequest("Invalid image format. Only JPG, JPEG, and PNG are allowed.");

                    var extensionGlb = Path.GetExtension(updated.PictureFileGlB.FileName).ToLower();
                    if (extensionGlb != ".glb")
                        return BadRequest("Invalid image format. Only GLB  are allowed.");

                    var file = Path.GetFileName(product.PictureUrl);
                    await bucket.Remove(new List<string> { $"Products/{Path.GetFileName(product.PictureUrl)}", $"Products/{Path.GetFileName(product.UrlGlb)}" });
                    
                    updated.PictureFile.CopyTo(ms);
                    await bucket.Upload(ms.ToArray(), product.Id + extension);

                    updated.PictureFileGlB.CopyTo(ms);
                    await bucket.Upload(ms.ToArray(), product.Id + extensionGlb);

                    product.PictureUrl = product.Id + extension;
                    product.UrlGlb = product.Id + extensionGlb;
                }

                _repos.Repo<Product>().Update(product);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500, $"An error occurred while saving the product: {ex.Message}, StackTrace: {ex.StackTrace}"));
            }
            var mapProductDTO = _mapper.Map<IEnumerable<ProductResponse>>(product, opt => {
                opt.Items["UserId"] = null;
            });
            return Ok(mapProductDTO);
        }

        [HttpPost]
        [Authorize("Admin")]
        [ProducesResponseType(typeof(ProductResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<ProductResponse>> PostProduct([FromForm] ProductRequest created)
        {
            if (created is null) return BadRequest(new ApiResponse(400));

            var param = new ProductSpecParams { SearchByName = created.Name };
            var spec = new ProductSpecific(param);
            var exists = await _repos.Repo<Product>().GetAllAsync(spec);
            if (exists.Any())
                return BadRequest(new ApiResponse(400, "Product already exists."));

            var productBrand = await _repos.Repo<ProductBrand>().GetByIdAsync(created.ProductBrandId);
            if (productBrand is null)
                return BadRequest(new ApiResponse(400, "Invalid ProductBrandId"));

            var productType = await _repos.Repo<ProductType>().GetByIdAsync(created.ProductTypeId);
            if (productType is null)
                return BadRequest(new ApiResponse(400, "Invalid ProductTypeId"));

            if (created.PictureFile == null || created.PictureFile.Length == 0)
                return BadRequest(new ApiResponse(400, "No image file provided."));

            if (created.PictureFileGlB == null || created.PictureFileGlB.Length == 0)
                return BadRequest(new ApiResponse(400, "No imageGLB file provided."));

            var extension = Path.GetExtension(created.PictureFile.FileName).ToLower();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                return BadRequest("Invalid image format. Only JPG, JPEG, and PNG are allowed.");

            var extensionGlb = Path.GetExtension(created.PictureFileGlB.FileName).ToLower();
            if (extensionGlb != ".glb")
                return BadRequest("Invalid image format. Only GLB  are allowed.");

            var lastId = (await _repos.Repo<Product>().GetAllAsync()).Max(p => p.Id) + 1;

            var product = new Product
            {
                Id = lastId,
                Name = created.Name,
                Description = created.Description,
                Price = created.Price,
                ProductBrandId = created.ProductBrandId,
                ProductTypeId = created.ProductTypeId,
                Quantity = created.Quantity,
                PictureUrl = lastId + extension,
                UrlGlb = lastId + extensionGlb,
            };

            try
            {
                using var ms = new MemoryStream();
                var bucket = _supabaseClient.Storage.From("Images/Products");
                created.PictureFile.CopyTo(ms);
                await bucket.Upload(ms.ToArray(), product.Id + extension);

                created.PictureFileGlB.CopyTo(ms);
                await bucket.Upload(ms.ToArray(), product.Id + extensionGlb);

                await _repos.Repo<Product>().AddAsync(product);
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500, ex.Message));
            }

            var mapProductDTO = _mapper.Map<IEnumerable<ProductResponse>>(product, opt => {
                opt.Items["UserId"] = null;
            });
            return Ok(mapProductDTO);
        }

        [HttpDelete("{id}")]
        [Authorize("Admin")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
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
                using var ms = new MemoryStream();
                var bucket = _supabaseClient.Storage.From("Images");

                _repos.Repo<Product>().Delete(product);
                await bucket.Remove(new List<string> { $"Products/{Path.GetFileName(product.PictureUrl)}", $"Products/{Path.GetFileName(product.UrlGlb)}" });
                await _repos.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _repos.DisposeAsync();
                return BadRequest(new ApiResponse(500, ex.Message));
            }
            return Ok(new ApiResponse(200, "Product deleted successfully"));
        }
    }
}
