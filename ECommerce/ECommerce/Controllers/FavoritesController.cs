using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Core.Models;
using ECommerce.Repo.Data;
using ECommerce.DTO;
using ECommerce.Errors;
using System.Security.Claims;
using AutoMapper;
using ECommerce.Core;
using ECommerce.Core.Specifications;
using ECommerce.Helper;
using Microsoft.AspNetCore.Authorization;
using ECommerce.DTO.Response;

namespace ECommerce.Controllers
{
    public class FavoritesController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitWork _repos;

        public FavoritesController(IMapper mapper, IUnitWork repos)
        {
            _mapper = mapper;
            _repos = repos;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Pagination<ProductResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<Pagination<ProductResponse>>> GetAllFavorites()
        {
            var param = new ProductSpecParams { isFav = true };
            var spec = new ProductSpecific(param);
            var favoriteProducts = await _repos.Repo<Product>().GetAllAsync(spec);
            if (favoriteProducts == null)
                return NotFound(new ApiResponse(404));

            var mappedProducts = _mapper.Map<IEnumerable<ProductResponse>>(favoriteProducts, opt => {
                opt.Items["UserId"] = null;
            });
            var countSpec = new ProductFilterationCount(param);
            var count = await _repos.Repo<Product>().GetCountAsync(countSpec);
            var pagination = new Pagination<ProductResponse>(param.PageIndex, param.PageSize, mappedProducts, count);
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<ProductResponse>> GetFavoritesById(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400, "Id is not valid."));

            var spec = new FavoriteSpec(id);
            var favorite = await _repos.Repo<Favorites>().GetByIdAsync(spec);
            if (favorite == null)
                return NotFound(new ApiResponse(404));

            var mappedFav = _mapper.Map<ProductResponse>(favorite, opt => {
                opt.Items["UserId"] = null;
            });
            return Ok(mappedFav);
        }

        [HttpPost("{productId}")]
        [Authorize("Sanctum")]
        [ProducesResponseType(typeof(FavoriteDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<FavoriteDTO>> AddOrRemoveFavorite(int productId)
        {
            if (productId <= 0) return BadRequest(new ApiResponse(400));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401));

            var spec = new FavoriteSpec(int.Parse(userId), productId);
            var existingFavorite = await _repos.Repo<Favorites>().GetByIdAsync(spec);
            if (existingFavorite != null)
            {
                existingFavorite.isFavorite = !existingFavorite.isFavorite;
                try
                {
                    _repos.Repo<Favorites>().Update(existingFavorite);
                    await _repos.CompleteAsync();
                }
                catch (Exception ex)
                {
                    await _repos.DisposeAsync();
                    return BadRequest(new ApiResponse(500, $"An error occurred while saving the product: {ex.Message}, StackTrace: {ex.StackTrace}"));
                }
                var mappedFav = _mapper.Map<FavoriteDTO>(existingFavorite, opt => {
                    opt.Items["UserId"] = userId;
                });
                return Ok(mappedFav);
            }

            var newFavorite = new Favorites { UserId = int.Parse(userId), ProductId = productId, isFavorite = true };
            await _repos.Repo<Favorites>().AddAsync(newFavorite);
            await _repos.CompleteAsync();

            var newmapped = _mapper.Map<FavoriteDTO>(newFavorite, opt => {
                opt.Items["UserId"] = userId;
            });
            return Ok(newmapped);
        }

        [HttpGet("user")]
        [Authorize("Sanctum")]
        [ProducesResponseType(typeof(ProductResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetUserFavorites()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401));

            var spec = new ProductSpecific(int.Parse(userId), "Favorite");
            var favorites = await _repos.Repo<Product>().GetAllAsync(spec);
            if (favorites == null)
                return NotFound(new ApiResponse(404, "No favorites found"));

            var newmapped = _mapper.Map<IEnumerable<ProductResponse>>(favorites, opt => {
                opt.Items["UserId"] = userId;
            });
            return Ok(newmapped);
        }
    }

}
