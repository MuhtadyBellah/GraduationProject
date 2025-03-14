using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public record BasketItemDTO
    {
        public int Id;
        public string Name;
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price Can't be Zero")]
        public decimal Price;
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity Must be One Item At Least")]
        public int Quantity;
        public string PictureUrl;
        public string UrlGlb;
        [Required]
        public string ProductBrand;
        [Required]
        public string ProductType;
    }
}