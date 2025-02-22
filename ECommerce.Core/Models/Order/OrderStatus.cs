using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Order
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "PaymentRecived")]
        PaymentRecived,
        [EnumMember(Value = "PaymentFailed")]
        PaymentFailed
    }
}
