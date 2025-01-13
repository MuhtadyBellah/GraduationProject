using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.DTO;

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
                .ForMember(d => d.UrlGlb, o => o.MapFrom<PictureGlb>());
        }
    }
}
