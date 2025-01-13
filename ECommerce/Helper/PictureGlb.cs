using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.DTO;

namespace ECommerce.Helper
{
    public class PictureGlb : IValueResolver<Product, ProductDTO, string>
    {
        private readonly IConfiguration _config;
        public PictureGlb(IConfiguration config) => _config = config;
        public string Resolve(Product source, ProductDTO destination, string destMember, ResolutionContext context)
            => (!string.IsNullOrEmpty(source.UrlGlb)) ? $"{_config["ApiBaseUrl"]}{source.UrlGlb}" : string.Empty;
    }
}
