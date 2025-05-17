using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Models
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
