using ECommerce.DTO.Response;

namespace ECommerce.DTO
{
    public class FavoriteDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? User { get; set; }
        public ProductResponse? Product { get; set; }
    }
}
