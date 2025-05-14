using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.DTO.Response;

namespace ECommerce.Helper
{
    public class PictureGlb : IValueResolver<Product, ProductResponse, string>
    {
        private readonly IConfiguration _config;
        public PictureGlb(IConfiguration config) => _config = config;
        public string Resolve(Product source, ProductResponse destination, string destMember, ResolutionContext context)
            => (!string.IsNullOrEmpty(source.UrlGlb)) ? $"{_config["ApiBaseUrl"]}{source.UrlGlb}" : string.Empty;
    }
}
