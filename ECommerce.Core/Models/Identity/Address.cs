using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Identity
{
    public class Address
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public required string City { get; set; }

        public string? Street { get; set; }

        public required string Country { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
