namespace ECommerce.DTO.Request
{
    public record MessageRequest(
        string UserDisplay,
        int ChatId,
        string Content
    );
}
