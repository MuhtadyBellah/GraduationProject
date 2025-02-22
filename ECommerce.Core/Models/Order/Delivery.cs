using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Order
{
    public class Delivery : BaseEntity
    {
        public Delivery(){}
        public Delivery(string sName, string? description, string dTime, decimal cost)
        {
            SName = sName;
            Description = description;
            DTime = dTime;
            Cost = cost;
        }

        public string SName { get; set; }
        public string? Description { get; set; }
        public string DTime { get; set; }
        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }
    }
}
