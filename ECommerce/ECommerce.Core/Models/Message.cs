using ECommerce.Core.Models.Laravel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Core.Models
{
    public class Message : BaseEntity
    {
        [Required]
        public string connectionId { get; set; }

        [ForeignKey(nameof(Chat))]
        public int? ChatId { get; set; }
        public Chat? Chat { get; set; }


        [ForeignKey(nameof(User))]
        public int SenderId { get; set; }
        public AppUser? User { get; set; }

        [Required]
        public string Content { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTimeOffset SentAt { get; set; } = DateTimeOffset.Now;
    }
}
