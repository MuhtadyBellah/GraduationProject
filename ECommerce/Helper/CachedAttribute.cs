using ECommerce.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace ECommerce.Helper
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _duration;

        public CachedAttribute(int duration)
        {
            _duration = duration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICache>();
            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
            var res = await cacheService.GetDataAsync(cacheKey);
            if (!string.IsNullOrEmpty(res))
            {
                var contentResult = new ContentResult()
                {
                    Content = res,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;
                return;
            }

            var exec = await next.Invoke(); // Execute Endpoint
            if (exec.Result is OkObjectResult okObjectResult && okObjectResult.Value != null)
            {
                await cacheService.CacheDataAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_duration));
            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
