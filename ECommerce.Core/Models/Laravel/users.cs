using ECommerce.Core.Models.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Laravel
{
    [Table("users")]
    public class Users
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        [Column("gender")]
        public string? Gender { get; set; }

        [Column("email_verified_at")]
        public DateTime? EmailVerifiedAt { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("google_id")]
        public string? GoogleId { get; set; }

        [Column("remember_token")]
        public string? RememberToken { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        
    }
}
