using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTO
{
    public record BasketItemDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price Can't be Zero")]
        public decimal Price { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity Must be One Item At Least")]
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }
        public string UrlGlb { get; set; }
        [Required]
        public string ProductBrand { get; set; }
        [Required] 
        public string ProductType { get; set; }
    }
}