using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public class OrderDTO
    {
        public string BasketId { get; set; }
        public int DeliveryMethodId { get; set; }
        public AddressDTO ShippingAddress { get; set; }
    }
}
