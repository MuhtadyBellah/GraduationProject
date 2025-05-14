namespace ECommerce.DTO.Request
{
    public record InvoiceRequest(decimal totalAmount, string address, string paymentMethod);
}
