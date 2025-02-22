using ECommerce.Core.Models.Order;

namespace ECommerce.DTO
{
    public class OrderReturnedDTO
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public string OrderDate { get; set; }
        public Address ShippingAddress { get; set; }
        public string Delivery { get; set; }
        public decimal DeliveryPrice { get; set; }
        public ICollection<OrderItemDTO> Items { get; set; } = new HashSet<OrderItemDTO>();
        public decimal SubTotal { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
