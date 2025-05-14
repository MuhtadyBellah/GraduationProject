using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Specifications
{
    public class deliverSpec : BaseSpecification<Delivery>
    {
        public deliverSpec(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.Orders);
        }
    }
}