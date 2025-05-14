namespace ECommerce.Core.Models
{
    public class ProductType : BaseEntity
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}
