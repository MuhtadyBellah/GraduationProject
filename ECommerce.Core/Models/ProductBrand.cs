namespace ECommerce.Core.Models
{
    public class ProductBrand : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
