using ECommerce.Core.Models.Laravel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models
{
    public class Favorites : BaseEntity
    {
        [Required]
        [ForeignKey("users")]
        public int UserId { get; set; }
        public Users? User { get; set; }

        public bool isFavorite { get; set; } = false;
        public bool isLike { get; set; } = false;

        [Required]
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
