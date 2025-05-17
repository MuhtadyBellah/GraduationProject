using ECommerce.Core.Models.Laravel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Core.Models.Order
{
    public class Invoice : BaseEntity
    {
        [Required]
        [ForeignKey(nameof(User))]
        public int userId { get; set; }
        public AppUser? User { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int orderId { get; set; }
        public Orders? Order { get; set; }

        public string InvoiceNumber { get; set; } = GenerateInvoiceNumber;
        public DateTime? InvoiceDate { get; set; }
        public bool IsPaid { get; set; } // order.Status == "paid"
        public decimal TotalAmount { get; set; } // delivery.cost + orders.total
        public string BillingAddress { get; set; } // Order.Address
        public string PaymentMethod {  get; set; } // Order.PaymentMethod
        public DateTime? PaymentDate { get; set; } // order.PaidAt
        public string? TransactionId { get; set; } // order.TransactionId

        public Invoice()
        {
            InvoiceNumber = GenerateInvoiceNumber;
            InvoiceDate = DateTime.Now;
        }

        public Invoice(int userId, int orderId, bool isPaid, decimal totalAmount, string billingAddress, string PaymentMethod, DateTime? paymentDate, string? transactionId)
        {
            InvoiceNumber = GenerateInvoiceNumber;
            InvoiceDate = DateTime.Now;
            this.userId = userId;
            this.orderId = orderId;
            IsPaid = isPaid;
            TotalAmount = totalAmount;
            BillingAddress = billingAddress;
            this.PaymentMethod = PaymentMethod;
            PaymentDate = paymentDate;
            TransactionId = transactionId;
        }

        private static string GenerateInvoiceNumber { get => "INV-#" + DateTimeOffset.UtcNow.ToString("yyyyMMddHHmm") + Guid.NewGuid().ToString("N")[..6].ToUpper(); }
    }

}
