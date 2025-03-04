namespace ECommerce.Errors
{
    internal class ApiException : ApiResponse
    {
        public string? detail { get; set; }
        public ApiException(int statusCode, string? message = null, string? v = null) : base(statusCode)
        {
            detail = v;
        }
    }
}