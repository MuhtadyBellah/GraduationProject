using ECommerce.Core.Models.Laravel;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Order
{
    public class Delivery : BaseEntity
    {
        public Delivery() { }
        public Delivery(string sName, string? description, string dTime, decimal cost, DeliveryStatus status = DeliveryStatus.Pending)
        {
            SName = sName;
            Description = description;
            DeliveryTime = dTime;
            Cost = cost;
            Status = status;
        }

        public string SName { get; set; }
        public string? Description { get; set; }
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
        public string DeliveryTime { get; set; }
        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }

        public ICollection<Orders> Orders { get; set; } = new List<Orders>();
    }
}
