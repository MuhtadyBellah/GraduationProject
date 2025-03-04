//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ECommerce.Core.Models.Order
//{
//    public class Order : BaseEntity
//    {
//        public Order() { }
//        public Order(string buyerEmail, Address shippingAddress, Delivery delivery, ICollection<OrderItem> items, decimal subTotal, string paymentIntentId)
//        {
//            BuyerEmail = buyerEmail;
//            ShippingAddress = shippingAddress;
//            Delivery = delivery;
//            Items = items;
//            SubTotal = subTotal;
//            PaymentIntentId = paymentIntentId;
//        }

//        public string BuyerEmail { get; set; }
//        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
//        public OrderStatus Status { get; set; } = OrderStatus.Pending;
//        public Address ShippingAddress { get; set; }
//        public Delivery Delivery { get; set; }
//        public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();
//        public decimal SubTotal { get; set; }
//        public decimal GetTotal() => SubTotal + Delivery.Cost;
//        public string PaymentIntentId { get; set; }
//    }
//}
