using ECommerce.Core.Models.Basket;
using ECommerce.Core.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Services
{
    public interface IPayment
    {
        Task<CustomerBasket?> CreatePaymentIntent(string basketId);
        Task<Order> PaymentSucceedOrFailed(string paymentIntentId, bool succeeded); 
        //Task<bool> ConfirmPaymentIntent();
        //Task<bool> CancelPaymentIntent();
    }
}
