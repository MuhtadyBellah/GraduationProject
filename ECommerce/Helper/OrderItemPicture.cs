//using AutoMapper;
//using ECommerce.Core.Models;
//using ECommerce.Core.Models.Order;
//using ECommerce.DTO;

//namespace ECommerce.Helper
//{
//    public class OrderItemPicture : IValueResolver<OrderItem, OrderItemDTO, string>
//    {
//        private readonly IConfiguration _config;
//        public OrderItemPicture(IConfiguration config) => _config = config;
//        public string Resolve(OrderItem source, OrderItemDTO destination, string destMember, ResolutionContext context)
//        => (!string.IsNullOrEmpty(source.Product.PictureUrl)) ? $"{_config["ApiBaseUrl"]}{source.Product.PictureUrl}" : string.Empty;
//    }
//}
