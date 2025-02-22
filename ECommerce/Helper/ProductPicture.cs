using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.DTO;

namespace ECommerce.Helper
{
    public class ProductPicture : IValueResolver<Product, ProductDTO, string>
    {
        private readonly IConfiguration _config;
        public ProductPicture(IConfiguration config) => _config = config;
        public string Resolve(Product source, ProductDTO destination, string destMember, ResolutionContext context)
            => (!string.IsNullOrEmpty(source.PictureUrl)) ? $"{_config["ApiBaseUrl"]}{source.PictureUrl}" : string.Empty;
    }
}
