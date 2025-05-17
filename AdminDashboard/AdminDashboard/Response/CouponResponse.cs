using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminDashboard
{
    public class CouponResponse
    {
        public List<CouponData> coupons { get; set; }
    }

    public class CouponData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string coupon_id { get; set; }
        public string Discount { get; set; }  // Stored as decimal (10.00 for 10%)
        public string Duration { get; set; }   // "once", "forever", etc.
        public bool is_used { get; set; }     // "Used", "Available"
        public string created_at { get; set; }
        public string updated_at { get; set; }
    }
}
