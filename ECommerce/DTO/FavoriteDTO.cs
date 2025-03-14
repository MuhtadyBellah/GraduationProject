using ECommerce.Core.Models.Laravel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.DTO
{
    public record FavoriteDTO
    {
        public int Id;
        public int UserId;
        public string? User;
        public bool isFavorite;
        public bool isLike;
        public int ProductId;
        public string? Product;
    }
}
