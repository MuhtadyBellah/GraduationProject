using System.Net;
using System.Text.Json;

namespace ECommerce.Errors
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleWare> log;
        private readonly IHostEnvironment env;

        public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> log, IHostEnvironment env)
        {
            this.next = next;
            this.log = log;
            this.env = env;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internal Server Error");
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
