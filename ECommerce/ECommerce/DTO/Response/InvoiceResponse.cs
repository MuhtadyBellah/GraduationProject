namespace ECommerce.DTO.Response
{
    public class InvoiceResponse
    {
        public int Id { get; set; }
        public string userId { get; set; }
        public string? User { get; set; }

        public int orderId { get; set; }
        public string? Order { get; set; }

        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public bool IsPaid { get; set; } // order.Status == "paid"
        public decimal TotalAmount { get; set; }  // delivery.cost + orders.total
        public string BillingAddress { get; set; } // Order.Address
        public string PaymentMethod { get; set; } // Order.PaymentMethod
        public string? PaymentDate { get; set; } // order.PaidAt
        public string? TransactionId { get; set; } // order.TransactionId
    }
}
