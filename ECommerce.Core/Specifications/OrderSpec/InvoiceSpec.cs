using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Specifications.OrderSpec
{
    public class InvoiceSpec : BaseSpecification<Invoice>
    {
        public InvoiceSpec(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.User);
            Includes.Add(p => p.Order);
        }
        public InvoiceSpec(string id) : base(p => p.userId == int.Parse(id))
        {
            Includes.Add(p => p.User);
            Includes.Add(p => p.Order);
        }
        public InvoiceSpec(List<int> ids) : base(p => ids.Contains(p.Id))
        {
            Includes.Add(p => p.User);
            Includes.Add(p => p.Order);
        }
    }
}
