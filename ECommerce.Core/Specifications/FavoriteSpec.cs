using ECommerce.Core.Models;
using ECommerce.Core.Models.Laravel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Specifications
{
    public class FavoriteSpec : BaseSpecification<Favorites>
    {
        public FavoriteSpec(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.User);
            Includes.Add(p => p.Product);
        }
        public FavoriteSpec(int userId, int productId) : base(p => p.Id == userId && p.ProductId == productId)
        {
            Includes.Add(p => p.User);
            Includes.Add(p => p.Product);
        }
        public FavoriteSpec(int userId, string s) : base(p => (s == "Favorite") ? p.isFavorite : p.isLike && p.Id == userId)
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
