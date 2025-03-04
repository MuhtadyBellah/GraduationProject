using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public record CustomerBasketDTO
    {
        [Required]
        public string Id { get; set; }
        public List<BasketItemDTO> Items { get; set; } = new List<BasketItemDTO>();
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public int? DeliveryId { get; set; }
    }   
}
