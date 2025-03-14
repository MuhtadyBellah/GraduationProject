namespace ECommerce.DTO
{
    public record MailRequestDTO
    {
        public string ToEmail;
        public string Subject;
        public string Body;
        public IList<IFormFile>? Files;
    }
}