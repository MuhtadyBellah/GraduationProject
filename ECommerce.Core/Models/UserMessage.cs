using ECommerce.Core.Models.Laravel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECommerce.Core.Models
{
    public record UserMessage
    {
        public string Id {  get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Message { get; set; }
        //public bool IsConnected { get; set; } 
        public DateTime DateSent { get; set; }
        public int UserId { get; set; }
        public Users? User { get; set; }
    }
}
