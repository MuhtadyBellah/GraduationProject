 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.Models.Basket
{
    public class CustomerBasket
    {
        public CustomerBasket(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
        public List<BasketItem> Items { set; get; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
        public int? DeliveryId { get; set; }
    }
}
