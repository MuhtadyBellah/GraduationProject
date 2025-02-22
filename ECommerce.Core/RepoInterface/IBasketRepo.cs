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
        Task<CustomerBasket?> GetBasketAsync(string basketId);
        Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBaketAsync(string basketId);

    }
}
