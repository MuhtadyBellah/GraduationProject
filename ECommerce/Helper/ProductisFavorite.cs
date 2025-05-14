using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.DTO.Response;

namespace ECommerce.Helper
{
    public class ProductisFavorite : IValueResolver<Product, ProductResponse, bool>
    {
        public bool Resolve(Product source, ProductResponse destination, bool destMember, ResolutionContext context)
        {
            var userId = context.Items["UserId"] as string;
            return (!string.IsNullOrEmpty(userId) ? source.Favorites.Any(f => f.isFavorite && f.UserId == int.Parse(userId))
                                                    : source.Favorites.Any(f => f.isFavorite));
        }
    }
}
