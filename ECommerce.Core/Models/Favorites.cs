using ECommerce.Core.Models.Laravel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Core.Models
{
    public class Favorites : BaseEntity
    {
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public AppUser? User { get; set; }

        public bool isFavorite { get; set; } = false;
        public bool isLike { get; set; } = false;

        [Required]
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
