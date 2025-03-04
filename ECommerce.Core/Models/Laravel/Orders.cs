using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ECommerce.Core.Models.Order;

namespace ECommerce.Core.Models.Laravel
{
    [Table("orders")]
    public class Orders
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("email")]
        [EmailAddress]
        public string Email { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("payment_method")]
        public string? PaymentMethod { get; set; }

        [Column("transaction_id")]
        public string? TransactionId { get; set; }

        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }

        public Users? User { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Delivery")]
        [Column("deliveryId")]
        public int deliveryId { get; set; }
        public Delivery? Delivery { get; set; }

        public Invoice? Invoice { get; set; }
    }
}
