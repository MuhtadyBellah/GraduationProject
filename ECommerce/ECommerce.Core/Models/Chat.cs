
using ECommerce.Core.Models.Laravel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Core.Models
{
    public class Chat : BaseEntity
    {
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; } // Sender
        public AppUser? Customer { get; set; }


        [ForeignKey(nameof(Agent))]
        public int? AgentId { get; set; } // Receiver
        public AppUser? Agent { get; set; }

        public string Category { get; set; } // Category of the chat
        public StatusOptions Status { get; set; } // Active, Closed, Pending
        public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? EndDate { get; set; }

        public ChatTicket? Ticket { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
    }
}
