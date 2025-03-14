using ECommerce.Core.Models.Laravel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.DTO
{
    public record InvoiceDTO
    {
        public int Id;
        public int userId;
        public string? User;

        public int orderId;
        public string? Order;

        public string InvoiceNumber;
        public string InvoiceDate;
        public bool IsPaid; // order.Status == "paid"
        public decimal TotalAmount; // delivery.cost + orders.total
        public string BillingAddress; // Order.Address
        public string PaymentMethod; // Order.PaymentMethod
        public string? PaymentDate; // order.PaidAt
        public string? TransactionId; // order.TransactionId
    }
}
