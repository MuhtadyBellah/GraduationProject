using ECommerce.Core.Models.Order;

namespace ECommerce.DTO
{
    public record DeliveryDTO
        (
        int Id,
        string SName,
        string? Description,
        string DeliveryTime,
        string Status,
        decimal Cost,
        decimal longLatiude,
        decimal lateLatiude
        )
    {
        public double? DistanceInKm { get; set; }
        public decimal? userOrdersTotal { get; set; }
        public decimal? totalSales { get; set; }
    }

}
