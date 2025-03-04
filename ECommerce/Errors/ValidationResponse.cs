namespace ECommerce.Errors
{
    public class ValidationResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; set; }
        public ValidationResponse() : base(400)
        {
        }
    }
}
