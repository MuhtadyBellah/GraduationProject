using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Specifications
{
    public enum SortOptions
    {
        [EnumMember(Value = "IdDesc")]
        IdDesc,
        [EnumMember(Value = "Name")]
        Name,
        [EnumMember(Value = "NameDesc")]
        NameDesc,
        [EnumMember(Value = "Price")]
        Price,
        [EnumMember(Value = "PriceDesc")]
        PriceDesc,
        [EnumMember(Value = "Brand")]
        Brand,
        [EnumMember(Value = "Type")]
        Type
    }
}
