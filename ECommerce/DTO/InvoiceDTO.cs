using ECommerce.Core.Models.Laravel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.DTO
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public string? User { get; set; }

        public int orderId { get; set; }
        public string? Order { get; set; }

        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public bool IsPaid { get; set; } // order.Status == "paid"
        public decimal TotalAmount { get; set; } // delivery.cost + orders.total
        public string BillingAddress { get; set; } // Order.Address
        public string PaymentMethod { get; set; } // Order.PaymentMethod
        public string? PaymentDate { get; set; } // order.PaidAt
        public string? TransactionId { get; set; } // order.TransactionId
    }
}
