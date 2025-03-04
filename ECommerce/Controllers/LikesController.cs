using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.Core.Specifications;
using ECommerce.Core;
using ECommerce.DTO;
using ECommerce.Errors;
using ECommerce.Helper;
using ECommerce.Repo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    public class LikesController : ApiBaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitWork _repos;
        
        public LikesController(IMapper mapper, IUnitWork repos)
        {
            _mapper = mapper;
            _repos = repos;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Pagination<ProductDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<Pagination<ProductDTO>>> GetAllLikes()
        {
            var param = new ProductSpecParams { isLike = true };
            var spec = new ProductSpecific(param);
            var LikeProducts = await _repos.Repo<Product>().GetAllAsync(spec);
            if (LikeProducts == null)
                return NotFound(new ApiResponse(404));
          
            var mappedProducts = _mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(LikeProducts);
            var countSpec = new ProductFilterationCount(param);
            var count = await _repos.Repo<Product>().GetCountAsync(countSpec);
            var pagination = new Pagination<ProductDTO>(param.PageIndex, param.PageSize, mappedProducts, count);
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FavoriteDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<FavoriteDTO>> GetLikesById(int id)
        {
            if (id <= 0) return BadRequest(new ApiResponse(400));

            var spec = new FavoriteSpec(id);
            var Like = await _repos.Repo<Favorites>().GetByIdAsync(spec);
            if (Like == null)
                return NotFound(new ApiResponse(404));

            var mappedLike = _mapper.Map<FavoriteDTO>(Like);
            return Ok(mappedLike);
        }

        [HttpPost("{productId}")]
        [Authorize(AuthenticationSchemes = "Sanctum")]
        [ProducesResponseType(typeof(FavoriteDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<FavoriteDTO>> AddOrRemoveLike(int productId)
        {
            if (productId <= 0) return BadRequest(new ApiResponse(400));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new ApiResponse(400, "User ID is null or empty"));

            var spec = new FavoriteSpec(int.Parse(userId), productId);
            var existingLike = await _repos.Repo<Favorites>().GetByIdAsync(spec);
            if (existingLike != null)
            {
                existingLike.isLike = !existingLike.isLike;
                try
                {
                    _repos.Repo<Favorites>().Update(existingLike);
                    await _repos.CompleteAsync();
                }
                catch (Exception ex)
                {
                    await _repos.DisposeAsync();
                    return BadRequest(new ApiResponse(500, $"An error occurred while saving the product: {ex.Message}, StackTrace: {ex.StackTrace}"));
                }
                var mappedLike = _mapper.Map<FavoriteDTO>(existingLike);
                return Ok(mappedLike);
            }

            var newLike = new Favorites { UserId = int.Parse(userId), ProductId = productId, isLike = true };
            await _repos.Repo<Favorites>().AddAsync(newLike);
            await _repos.CompleteAsync();

            var newmapped = _mapper.Map<FavoriteDTO>(newLike);
            return Ok(newmapped);
        }

        [HttpGet("user/{userId}")]
        [Authorize(AuthenticationSchemes = "Sanctum")]
        [ProducesResponseType(typeof(Favorites), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<IEnumerable<FavoriteDTO>>> GetUserLikes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new ApiResponse(400, "User ID is null or empty"));

            var spec = new FavoriteSpec(int.Parse(userId), "Like");
            var Likes = await _repos.Repo<Favorites>().GetAllAsync(spec);
            if (Likes == null) 
                return NotFound(new ApiResponse(404, "No Likes found"));

            var newmapped = _mapper.Map<IEnumerable<Favorites>, IEnumerable<FavoriteDTO>>(Likes);
            return Ok(newmapped);
        }
    }
}
