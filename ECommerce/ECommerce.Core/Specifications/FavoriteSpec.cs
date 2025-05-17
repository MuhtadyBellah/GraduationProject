using ECommerce.Core.Models;

namespace ECommerce.Core.Specifications
{
    public class FavoriteSpec : BaseSpecification<Favorites>
    {
        public FavoriteSpec(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.User);
            Includes.Add(p => p.Product);
        }
        public FavoriteSpec(int userId, int productId) : base(p => p.UserId == userId && p.ProductId == productId)
        {
            Includes.Add(p => p.User);
            Includes.Add(p => p.Product);
        }
        public FavoriteSpec(int userId, string s) : base(p => (s == "Favorite") ? p.isFavorite && p.UserId == userId : p.isLike && p.UserId == userId)
        {
            Includes.Add(p => p.User);
            Includes.Add(p => p.Product);
        }
        public FavoriteSpec(List<int> ids) : base(p => ids.Contains(p.Id))
        {
            Includes.Add(p => p.User);
            Includes.Add(p => p.Product);
        }
    }
}
