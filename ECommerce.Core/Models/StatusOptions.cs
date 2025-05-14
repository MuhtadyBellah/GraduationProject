using System.Runtime.Serialization;

namespace ECommerce.Core.Models
{
    public enum StatusOptions
    {
        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "Active")]
        Active,
        [EnumMember(Value = "Closed")]
        Closed
    }
}
