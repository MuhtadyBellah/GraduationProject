
namespace ECommerce.Errors
{
    public class ApiResponse
    {
        public int StatCode { get; set; }
        public string? Message { get; set; }
        public ApiResponse(int statCode, string? message = null)
        {
            StatCode = statCode;
            Message = message ?? GetDefaultMessage(StatCode);
        }

        private string? GetDefaultMessage(int? statCode)
        {
            return statCode switch
            {
                400 => "A bad request, you have made",
                401 => "Authorized, you are not",
                404 => "Resource Not found",
                500 => "Errors are the path to the dark side. Errors lead to anger. Anger leads to hate. Hate leads to career change",
                _ => null
            };
        }
    }
}
