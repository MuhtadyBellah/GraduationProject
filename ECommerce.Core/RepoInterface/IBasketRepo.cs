using ECommerce.Core.Models.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.RepoInterface
{
    public interface IBasketRepo
    {
        Task<CustomerBasket?> GetBaketAsync(string basketId);
        Task<CustomerBasket?> UpdateBaketAsync(CustomerBasket basket);
        Task<bool> DeleteBaketAsync(string basketId);

    }
}
