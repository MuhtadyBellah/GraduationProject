using ECommerce.Core.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Specifications.OrderSpec
{
    public class OrderSpec : BaseSpecification<Order>
    {

        public OrderSpec(string buyerEmail) : base(o => o.BuyerEmail == buyerEmail)
        {
            Includes.Add(o => o.Delivery);
            Includes.Add(o => o.Items);
            OrderByDesc(o => o.OrderDate);
        }
        public OrderSpec(int orderId, string buyerEmail) : base(o => o.Id == orderId && o.BuyerEmail == buyerEmail)
        {
            Includes.Add(o => o.Delivery);
            Includes.Add(o => o.Items);
            OrderByDesc(o => o.OrderDate);
        }
    }
}
