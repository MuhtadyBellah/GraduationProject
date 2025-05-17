using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Core.Services;
using ECommerce.Core.Specifications;
using ECommerce.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using System.Text.Json;

namespace ECommerce.Controllers
{
    public class CacheController : ApiBaseController
    {
        private readonly ICache _cacheService;
        private readonly IUnitWork _unitWork;
        private readonly string _cacheKey = "AR:";
        private readonly List<string> _items;

        public CacheController(ICache _cacheService, IUnitWork unitWork)
        {
            this._cacheService = _cacheService;
            this._unitWork = unitWork;
            _items = new List<string>()
            {
                "https://mywebar.com/p/Project_0_9hewmshcte", // 1
                "https://mywebar.com/p/Project_2_9qqev5b6y3", // 2
                "https://mywebar.com/p/Project_3_i0iyw5d1pf", // 3
                "https://mywebar.com/p/Project_4_x7shj5mswy", // 4
                "https://mywebar.com/p/Project_1_pz9bxsrvo4" , // 5
                "https://mywebar.com/p/Project_8_8b2e3qi5jw", // 6
                "https://mywebar.com/p/Project_6_375p3v4fwb", // 7
                "https://mywebar.com/p/Project_10_amcv4r43zl", // 8
                "https://mywebar.com/p/Project_9_g1lucqvv3e", // 9
                "https://mywebar.com/p/Project_7_t3w74nprur", // 10
            };
        }

        public record AR(int id, string link);

        [HttpPost("AR")]
        public async Task<IActionResult> SetCache()
        {
            var spec = new ProductSpecific(new ProductSpecParams {PageSize = 10 });
            var products = await _unitWork.Repo<Product>().GetAllAsync(spec);
           
            var i = 0;
            var tasks = new List<Task>();
            foreach (var product in products)
            {
                if (i >= _items.Count) break;

                tasks.Add( 
                    _cacheService.CacheDataAsync(_cacheKey + product.Id, _items[i++], TimeSpan.FromDays(5))
                    );
            }
            await Task.WhenAll(tasks);
            return Ok();
        }

        [HttpGet("AR/{productId}")]
        public async Task<IActionResult> GetCache(int productId)
        {
            var cachedValue = await _cacheService.GetDataAsync(_cacheKey + productId);
            if (cachedValue == null)
                return NotFound(new ApiResponse(404));

            var data = JsonSerializer.Deserialize<string>(cachedValue);
            return Ok(data);
        }

        [HttpGet("{key}/{value}/{expireSeconds}")]
        public async Task<IActionResult> SetCache(string key, string value, int expireSeconds)
        {
            if (string.IsNullOrEmpty(key) || expireSeconds <= 0)
                return BadRequest(new ApiResponse(400, "Invalid key, value, or expire time."));

            key = "Front:" + key;
            await _cacheService.CacheDataAsync(key, value, TimeSpan.FromSeconds(expireSeconds));
            return Ok($"Cached key '{key}' with expiration {expireSeconds} seconds.");
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetCache(string key)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest(new ApiResponse(400, "Key is required."));
            
            key = $"Front:{key}";
            var data = await _cacheService.GetDataAsync(key);
            if (data == null)
                return NotFound(new ApiResponse(404, $"No data found for key '{key}'."));

            var res = JsonSerializer.Deserialize<string>(data);
            return Ok(res);
        }
    }
}
