using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Specifications
{
    public class InvoiceSpec : BaseSpecification<Invoice>
    {
        public InvoiceSpec(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.User);
            Includes.Add(p => p.Order);
        }
        public InvoiceSpec(int UserId, string? s = "user") : base(p => p.userId == UserId)
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
