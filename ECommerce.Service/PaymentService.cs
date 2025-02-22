using ECommerce.Core;
using ECommerce.Core.Models.Basket;
using ECommerce.Core.Models.Order;
using ECommerce.Core.RepoInterface;
using ECommerce.Core.Services;
using ECommerce.Core.Specifications.OrderSpec;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Service
{
    public class PaymentService : IPayment
    {
        private readonly IConfiguration _config;
        private readonly IBasketRepo _basketRepo;
        private readonly IUnitWork _repos;

        public PaymentService(IConfiguration config, IBasketRepo basketRepo, IUnitWork repos)
        {
            this._config = config;
            this._basketRepo = basketRepo;
            this._repos = repos;
        }
        //public Task<bool> CancelPaymentIntent()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> ConfirmPaymentIntent()
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<CustomerBasket?> CreatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
            //Get Basket
            var basket = await _basketRepo.GetBasketAsync(basketId);
            if (basket is null) return null;

            decimal amount = 0;
            if (basket.DeliveryId.HasValue)
            {
                var delivery = await _repos.Repo<Delivery>().GetByIdAsync(basket.DeliveryId.Value);
                if (delivery != null) amount = delivery.Cost;
            }

            if (basket.Items.Count > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await _repos.Repo<Core.Models.Product>().GetByIdAsync(item.Id);
                    if (product != null) item.Price = product.Price;
                }
            }

            amount += basket.Items.Sum(x => x.Price * x.Quantity);

            var service = new PaymentIntentService();
            PaymentIntent paymentIntent;
            if (string.IsNullOrEmpty(basket.PaymentIntentId)) // Create
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)amount * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                paymentIntent = await service.CreateAsync(options);
                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else //Update
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)amount * 100
                };
                paymentIntent = await service.UpdateAsync(basket.PaymentIntentId, options);
                basket.ClientSecret = paymentIntent.ClientSecret;
            }
            await _basketRepo.UpdateBasketAsync(basket);

            return basket;
        }

        public async Task<Order> PaymentSucceedOrFailed(string paymentIntentId, bool succeeded)
        {
            var spec = new OrderPaymentSpec(paymentIntentId);
            var order = await _repos.Repo<Order>().GetEntityAsync(spec);
            if(succeeded) order.Status = OrderStatus.PaymentRecived;
            else order.Status = OrderStatus.PaymentFailed;
            _repos.Repo<Order>().Update(order);
            await _repos.CompleteAsync();
            return order;
        }
    }
}
