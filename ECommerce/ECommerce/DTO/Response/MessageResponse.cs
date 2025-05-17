namespace ECommerce.DTO.Response
{
    public record MessageResponse(
        string connectionId,
        string UserName,
        string Message,
        string DateSent,
        string UserId
    );
}
