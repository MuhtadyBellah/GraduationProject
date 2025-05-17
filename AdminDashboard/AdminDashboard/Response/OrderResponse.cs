using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminDashboard
{
    public class OrderResponse
    {
        public List<OrderData> orders { get; set; }
    }

    public class OrderData
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string total { get; set; }
        public string status { get; set; }
        public string payment_method { get; set; }
        public string transaction_id { get; set; }
        public string paid_at { get; set; }
        public string note { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int user_id { get; set; }
        public int? deliveryId { get; set; } = null;
    }
}
