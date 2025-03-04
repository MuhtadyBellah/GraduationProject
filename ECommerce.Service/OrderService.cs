//using ECommerce.Core;
//using ECommerce.Core.Models;
//using ECommerce.Core.Models.Order;
//using ECommerce.Core.RepoInterface;
//using ECommerce.Core.Repos;
//using ECommerce.Core.Services;
//using ECommerce.Core.Specifications.OrderSpec;

//namespace ECommerce.Service
//{
//    public class OrderService : IOrder
//    {
//        private readonly IBasketRepo _basket;
//        private readonly IUnitWork _repos;
//        private readonly IPayment _paymentService;

//        public OrderService(IBasketRepo basketRepo, IUnitWork repos, IPayment paymentService)
//        {
//            _basket = basketRepo;
//            _repos = repos;
//            this._paymentService = paymentService;
//        }

//        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryId, Address shippingAddress)
//        {
//            //Get Basket
//            var basket = await _basket.GetBasketAsync(basketId);
//            if (basket == null) return null; // Ensure basket is not null

//            //Get Items
//            var orderItems = new List<OrderItem>();
//            if (basket.Items.Count > 0)
//            {
//                foreach (var item in basket.Items)
//                {
//                    var productItem = await _repos.Repo<Product>().GetByIdAsync(item.Id);
//                    if (productItem == null) continue; // Ensure productItem is not null
//                    var productOrdered = new ProductOrdered(productItem.Id, productItem.Name, productItem.PictureUrl, productItem.UrlGlb);
//                    var orderItem = new OrderItem(productOrdered, item.Quantity, item.Price);
//                    orderItems.Add(orderItem);
//                }
//            }

//            //Calc SubTotal
//            var subtotal = orderItems.Sum(item => item.Price * item.Quantity);

//            //Get Delivery 
//            var delivery = await _repos.Repo<Delivery>().GetByIdAsync(deliveryId);
//            if (delivery == null) return null; // Ensure delivery is not null

//            //Create Order
//            if (basket.PaymentIntentId == null) return null; // Ensure PaymentIntentId is not null
//            var spec = new OrderPaymentSpec(basket.PaymentIntentId);
//            var ExOrder = await _repos.Repo<Order>().GetEntityAsync(spec);
//            if (ExOrder is not null)
//            {
//                _repos.Repo<Order>().Delete(ExOrder);
//                await _paymentService.CreatePaymentIntent(basketId);
//                await _repos.CompleteAsync();
//            }
//            var order = new Order(buyerEmail, shippingAddress, delivery, orderItems, subtotal, basket.PaymentIntentId);

//            //Add Order to local db
//            await _repos.Repo<Order>().AddAsync(order);

//            //Save to db
//            var res = await _repos.CompleteAsync();
//            return (res <= 0) ? null : order;
//        }

//        public async Task<IEnumerable<Order>> GetAllOrdersAsync(string buyerEmail)
//        {
//            var spec = new OrderSpec(buyerEmail);
//            var orders = await _repos.Repo<Order>().GetAllAsync(spec);
//            return orders;
//        }

//        public async Task<Order?> GetOrderByIdforUserAsync(string buyerEmail, int orderId)
//        {
//            var spec = new OrderSpec(orderId, buyerEmail);
//            var orders = await _repos.Repo<Order>().GetEntityAsync(spec);
//            return orders;
//        }
//    }
//}
