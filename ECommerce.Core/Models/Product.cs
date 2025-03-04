using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Core.Models
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string PictureUrl { get; set; }
        public required string UrlGlb { get; set; }
        public decimal Price { get; set; }

        // Foreign Key for ProductBrand
        public int ProductBrandId { get; set; }
        public ProductBrand? ProductBrand { get; set; }

        // Foreign Key for ProductType
        public int ProductTypeId { get; set; }
        public ProductType? ProductType { get; set; }

        public int Quantity { get; set; }

        public ICollection<Favorites> Favorites { get; set; } = new List<Favorites>();
    }
}
