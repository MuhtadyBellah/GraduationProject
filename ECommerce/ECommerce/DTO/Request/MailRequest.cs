namespace ECommerce.DTO.Request
{
    public record MailRequest
    (
        string ToEmail,
        string Subject,
        string Body,
        IList<IFormFile>? Files
    );
}