using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.Core.Repos;
using ECommerce.Core.Specifications;
using ECommerce.DTO;
using ECommerce.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    public class ProductController : ApiBaseController
    {
        private readonly IGenericRepo<Product> _context;
        private readonly IGenericRepo<ProductType> _contextType;
        private readonly IGenericRepo<ProductBrand> _contextBrand;
        private readonly IMapper _mapper;

        public ProductController(IGenericRepo<Product> context, IMapper mapper, IGenericRepo<ProductType> contextType, IGenericRepo<ProductBrand> contextBrand)
        {
            _context = context;
            _mapper = mapper;
            _contextType = contextType;
            _contextBrand = contextBrand;
        }

        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductDTO>>> GetProducts([FromQuery] ProductSpecParams param)
        {
            var spec = new ProductSpecific(param);
            var products = await _context.GetAllAsync(spec);
            var mapProducts = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(products);
            var CountSpec = new ProductFilterationCount(param);
            var count = await _context.GetCountAsync(CountSpec);

            return Ok(new Pagination<ProductDTO>(param.PageIndex, param.PageSize, mapProducts, count));
        }

        //[HttpGet("Favorite")]
        //public async Task<ActionResult<Pagination<ProductDTO>>> GetFavorites([FromQuery] ProductSpecParams param)
        //{
        //    var spec = new ProductSpecific(param);
        //    var products = await _context.GetAllAsync(spec);
        //    var mapProducts = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(products);
        //    var CountSpec = new ProductFilterationCount(param);
        //    var count = await _context.GetCountAsync(CountSpec);

        //    return Ok(new Pagination<ProductDTO>(param.PageIndex, param.PageSize, mapProducts, count));
        //}

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDTO), 200)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProductById(int id)
        {
            if (id <= 0) return BadRequest("Id is not valid.");

            var spec = new ProductSpecific(id);
            var product = await _context.GetByIdAsync(spec);
            var MapProduct = _mapper.Map<Product, ProductDTO>(product);
            if (MapProduct is null) return NotFound();
            return Ok(MapProduct);
        }

        
        [HttpGet("{id}/img")]
        public async Task<ActionResult> GetItemPictureById(int id)
        {
            var spec = new ProductSpecific(id);
            var product = await _context.GetByIdAsync(spec);
            var MapProduct = _mapper.Map<Product, ProductDTO>(product);
            if (MapProduct is null) return NotFound();
            return Redirect(MapProduct.PictureUrl);
        }

        [HttpGet("{id}/imgGlb")]
        public async Task<IActionResult> GetItemPictureGlb(int id)
        {
            var spec = new ProductSpecific(id);
            var product = await _context.GetByIdAsync(spec);
            var MapProduct = _mapper.Map<Product, ProductDTO>(product);
            var fileUrl = MapProduct.UrlGlb;
            if (string.IsNullOrEmpty(fileUrl))
                return NotFound(new { Message = "File URL not found." });

            return Ok(fileUrl.ToString());
        }

        /* Put & Post
        [HttpPut]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> PutProduct(ProductParams updated)
        {
            var product = await _context.GetByIdAsync(updated.id);
            if (product is null)
                return NotFound();

            var file = Path.GetFileName(updated.PictureUrl);
            if (Path.GetExtension(file).ToLower() == ".jpg" ||
                Path.GetExtension(file).ToLower() == ".jpeg" ||
                Path.GetExtension(file).ToLower() == ".png" ||
                Path.GetExtension(file).ToLower() == ".gif")
            {
                var res = "Images/Products/" + file;
                product.PictureUrl = res;  
            }
            else return BadRequest("Invalid image format. Only JPG, JPEG, PNG, and GIF are allowed."); ;

            product.Name = updated.Name;
            product.Description = updated.Description;
            product.Price = updated.Price;
            product.ProductBrandId = updated.ProductBrandId;
            product.ProductTypeId = updated.ProductTypeId;
            product.Quantity = updated.Quantity;
            product.ProductBrand =
            product.ProductType =

            try
            {
                await _context.UpdateAsync(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while saving the product.");
            }

            var mapProductDTO = _mapper.Map<Product, ProductDTO>(product);
            return Ok(mapProductDTO);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> PostProduct(ProductParams created)
        {
            if (created is null) return BadRequest();
            var product = new Product
            {
                Name = created.Name,
                Description = created.Description,
                Price = created.Price,
                ProductBrandId = created.ProductBrandId,
                ProductTypeId = created.ProductTypeId,
                Quantity = created.Quantity,
            };

            var file = Path.GetFileName(created.PictureUrl);
            if (Path.GetExtension(file).ToLower() == ".jpg" ||
                Path.GetExtension(file).ToLower() == ".jpeg" ||
                Path.GetExtension(file).ToLower() == ".png" ||
                Path.GetExtension(file).ToLower() == ".gif")
            {
                var res = "Images/Products/" + file;
                product.PictureUrl = res;
            }
            else return BadRequest("Invalid image format. Only JPG, JPEG, PNG, and GIF are allowed.");

            try
            {
                await _context.CreateAsync(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while saving the product: {ex.Message}, StackTrace: {ex.StackTrace}");
            }

            var mapProductDTO = _mapper.Map<Product, ProductDTO>(product);
            return Ok(mapProductDTO);
        }
        */
        /* DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

        [HttpGet("Categories")]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductTypes()
            => Ok(await _contextType.GetAllAsync());

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

        [HttpGet("Brands")]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> GetProductBrands()
            => Ok(await _contextBrand.GetAllAsync());

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
