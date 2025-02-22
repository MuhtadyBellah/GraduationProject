using ECommerce.Core.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services
{
    public interface IOrder
    {
        Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryId, Address shippingAddress);
        Task<IEnumerable<Order>> GetAllOrdersAsync(string buyerEmail);
        Task<Order?> GetOrderByIdforUserAsync(string buyerEmail, int orderId);
    }
}
