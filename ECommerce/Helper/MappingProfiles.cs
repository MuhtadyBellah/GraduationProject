using AutoMapper;
using ECommerce.DTO;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using ECommerce.DTO.Response;

namespace ECommerce.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Product, ProductResponse>()
                .ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPicture>())
                .ForMember(d => d.UrlGlb, o => o.MapFrom<PictureGlb>())
                .ForMember(dest => dest.isFav, opt => opt.MapFrom<ProductisFavorite>())
                .ForMember(dest => dest.isLike, opt => opt.MapFrom<ProductisLike>());

            CreateMap<Favorites, FavoriteDTO>()
                .ForMember(d => d.User, o => o.MapFrom(s => s.User.Name))
                .ForMember(d => d.Product, o => o.MapFrom(s => s.Product));

            CreateMap<Invoice, InvoiceResponse>()
                .ForMember(d => d.User, o => o.MapFrom(s => s.User.Name))
                .ForMember(d => d.Order, o => o.MapFrom(s => s.Order.Email));

            CreateMap<Delivery, DeliveryDTO>();
        }
    }
}
