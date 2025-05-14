using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Core.Models
{
    public class ChatTicket : BaseEntity
    {
        public string TicketNumber { get; set; }

        [Required]
        [ForeignKey(nameof(Chat))]
        public int ChatId { get; set; }
        public Chat? Chat { get; set; }
        public string Topic { get; set; }

        public string? Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}