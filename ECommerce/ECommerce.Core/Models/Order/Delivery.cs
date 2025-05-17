using System.ComponentModel.DataAnnotations;
using ECommerce.Core.Models.Laravel;

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

        [DataType(DataType.Currency)]
        public decimal longLatiude;
        [DataType(DataType.Currency)]
        public decimal lateLatiude;

        public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
    }
}
