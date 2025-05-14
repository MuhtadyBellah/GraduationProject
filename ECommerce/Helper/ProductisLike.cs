using AutoMapper;
using ECommerce.Core.Models;
using ECommerce.DTO.Response;
using Supabase.Gotrue;

namespace ECommerce.Helper
{
    public class ProductisLike : IValueResolver<Product, ProductResponse, bool>
    {
        public bool Resolve(Product source, ProductResponse destination, bool destMember, ResolutionContext context)
        {
            var userId = context.Items["UserId"] as string;
            return (!string.IsNullOrEmpty(userId) ? source.Favorites.Any(f => f.isLike && f.UserId == int.Parse(userId))
                                                    : source.Favorites.Any(f => f.isLike));
        }
    }
}