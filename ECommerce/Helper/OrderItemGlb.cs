//using AutoMapper;
//using ECommerce.Core.Models.Order;
//using ECommerce.DTO;

//namespace ECommerce.Helper
//{
//    public class OrderItemGlb : IValueResolver<OrderItem, OrderItemDTO, string>
//    {
//        private readonly IConfiguration _config;
//        public OrderItemGlb(IConfiguration config) => _config = config;
//        public string Resolve(OrderItem source, OrderItemDTO destination, string destMember, ResolutionContext context)
//        => (!string.IsNullOrEmpty(source.Product.UrlGlb)) ? $"{_config["ApiBaseUrl"]}{source.Product.UrlGlb}" : string.Empty;
//    }
//}
