using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.DTO.Response;

namespace ECommerce.Helper
{
    public class ProductPicture : IValueResolver<Product, ProductResponse, string>
    {
        private readonly IConfiguration _config;
        public ProductPicture(IConfiguration config) => _config = config;
        public string Resolve(Product source, ProductResponse destination, string destMember, ResolutionContext context)
            => (!string.IsNullOrEmpty(source.PictureUrl)) ? $"{_config["ApiBaseUrl"]}{source.PictureUrl}" : string.Empty;

        //https://bazvfoiiqfamubdjqgoi.supabase.co/storage/v1/object/public/Images/Products/1.png
    }
}
