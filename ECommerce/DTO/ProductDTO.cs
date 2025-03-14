using System.Diagnostics.CodeAnalysis;

namespace ECommerce.DTO
{
    public record ProductDTO
    {
        public int Id;
        public string Name;
        public string? Description;
        public string PictureUrl;
        public string UrlGlb;
        public decimal Price;
        public int ProductBrandId;
        public string? ProductBrand;
        public int ProductTypeId;
        public string? ProductType;
        public int Quantity;
        public bool isFavorite = false;
        public bool isLike = false;
    }
}
