using System.Runtime.Serialization;

namespace ECommerce.Core.Models.Order
{
    public enum DeliveryStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "Delivered")]
        Delivered
    }
}