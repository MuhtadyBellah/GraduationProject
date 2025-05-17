namespace ECommerce.DTO.Request
{
    public class ProductRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int ProductBrandId { get; set; }
        public int ProductTypeId { get; set; }
        public int Quantity { get; set; }
        public IFormFile? PictureFile { get; set; }
        public IFormFile? PictureFileGlB { get; set; }
    }
}
