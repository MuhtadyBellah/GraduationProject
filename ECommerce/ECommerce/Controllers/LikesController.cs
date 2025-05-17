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
using Supabase.Gotrue;
using ECommerce.DTO.Response;

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
        [ProducesResponseType(typeof(Pagination<ProductResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<Pagination<ProductResponse>>> GetAllLikes()
        {

            var param = new ProductSpecParams { isLike = true };
            var spec = new ProductSpecific(param);
            var LikeProducts = await _repos.Repo<Product>().GetAllAsync(spec);
            if (LikeProducts == null)
                return NotFound(new ApiResponse(404));

            var mappedProducts = _mapper.Map<IEnumerable<ProductResponse>>(LikeProducts, opt => {
                opt.Items["UserId"] = null;
            });
            var countSpec = new ProductFilterationCount(param);
            var count = await _repos.Repo<Product>().GetCountAsync(countSpec);
            var pagination = new Pagination<ProductResponse>(param.PageIndex, param.PageSize, mappedProducts, count);
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

            var mappedLike = _mapper.Map<FavoriteDTO>(Like, opt => {
                opt.Items["UserId"] = null;
            });
            return Ok(mappedLike);
        }

        [HttpPost("{productId}")]
        [Authorize]
        [ProducesResponseType(typeof(FavoriteDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 500)]
        public async Task<ActionResult<FavoriteDTO>> AddOrRemoveLike(int productId)
        {
            if (productId <= 0) return BadRequest(new ApiResponse(400));

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401));

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
                var mappedLike = _mapper.Map<FavoriteDTO>(existingLike, opt => {
                    opt.Items["UserId"] = userId;
                });
                return Ok(mappedLike);
            }

            var newLike = new Favorites { UserId = int.Parse(userId), ProductId = productId, isLike = true };
            await _repos.Repo<Favorites>().AddAsync(newLike);
            await _repos.CompleteAsync();

            var newmapped = _mapper.Map<FavoriteDTO>(newLike, opt => {
                opt.Items["UserId"] = userId;
            });
            return Ok(newmapped);
        }

        [HttpGet("user")]
        [Authorize("Sanctum")]
        [ProducesResponseType(typeof(ProductResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 401)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetUserLikes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse(401));

            var spec = new ProductSpecific(int.Parse(userId), "Like");
            var Likes = await _repos.Repo<Product>().GetAllAsync(spec);
            if (Likes == null)
                return NotFound(new ApiResponse(404, "No Likes found"));

            var newmapped = _mapper.Map<IEnumerable<ProductResponse>>(Likes, opt => {
                opt.Items["UserId"] = userId;
            });
            return Ok(newmapped);
        }
    }
}
