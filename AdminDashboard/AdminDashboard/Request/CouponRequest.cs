using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminDashboard.Request
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace AdminDashboard.Request
    {
        public class CouponRequest
        {
            public string name { get; set; }
            public string duration { get; set; }
            public string percent_off { get; set; }
            public string coupon_number { get; set; }
        }
    }

}
