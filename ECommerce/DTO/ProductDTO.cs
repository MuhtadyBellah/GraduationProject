namespace ECommerce.DTO
{
    public class ProductDTO
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string PictureUrl { get; set; }
        public string UrlGlb { get; set; }
        public decimal Price { get; set; }
        public int ProductBrandId { get; set; }
        public string ProductBrand { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductType { get; set; }
        public int Quantity { get; set; }
        //public bool isFav { get; set; }
    }
}
