using ECommerce.Core.Models.Laravel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.DTO
{
    public class FavoriteDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? User { get; set; }
        public bool isFavorite { get; set; } = false;
        public bool isLike { get; set; } = false;
        public int ProductId { get; set; }
        public string? Product { get; set; }
    }
}
