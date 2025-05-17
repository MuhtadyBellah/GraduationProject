namespace ECommerce.DTO.Request
{
    public record TicketRequest
    (
        string TicketNumber,
        string Topic,
        string Description,
        int ChatId
    );
}
