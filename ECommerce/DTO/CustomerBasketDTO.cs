using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public record CustomerBasketDTO
    {
        [Required]
        public string Id;
        public List<BasketItemDTO> Items;
        public string? PaymentIntentId;
        public string? ClientSecret;
        public int? DeliveryId;
    }   
}
