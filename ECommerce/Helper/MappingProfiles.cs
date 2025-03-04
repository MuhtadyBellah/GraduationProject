using AutoMapper;
using ECommerce.Core.Models.Basket;
using ECommerce.DTO;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;

namespace ECommerce.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPicture>())
                .ForMember(d => d.UrlGlb, o => o.MapFrom<PictureGlb>())
                .ForMember(d => d.isFavorite, o => o.MapFrom(s => s.Favorites.Any(f => f.isFavorite)))
                .ForMember(d => d.isLike, o => o.MapFrom(s => s.Favorites.Any(f => f.isLike)));


            CreateMap<CustomerBasket, CustomerBasketDTO>().ReverseMap();
            CreateMap<BasketItem, BasketItemDTO>().ReverseMap();

            CreateMap<Favorites, FavoriteDTO>()
                .ForMember(d => d.User, o => o.MapFrom(s => s.User.Name))
                .ForMember(d => d.Product, o => o.MapFrom(s => s.Product.Name));

            CreateMap<Invoice, InvoiceDTO>()
                .ForMember(d => d.User, o => o.MapFrom(s => s.User.Name))
                .ForMember(d => d.Order, o => o.MapFrom(s => s.Order.Email));

            /*OrderMapping
            CreateMap<Core.Models.Order.Address, AddressDTO>().ReverseMap();

            CreateMap<Order, OrderReturnedDTO>()
                .ForMember(d => d.Delivery, o => o.MapFrom(s => s.Delivery.SName))
                .ForMember(d => d.DeliveryPrice, o => o.MapFrom(s => s.Delivery.Cost));

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.Product.PictureUrl))
                .ForMember(d => d.UrlGlb, o => o.MapFrom(s => s.Product.UrlGlb))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemPicture>())
                .ForMember(d => d.UrlGlb, o => o.MapFrom<OrderItemGlb>());
            */
            //CreateMap<Core.Models.Identity.Address, AddressDTO>().ReverseMap();
        }
    }
}
