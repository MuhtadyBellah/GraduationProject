using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminDashboard.Request
{
    public class MessageRequest
    {
        public string UserDisplay { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }
    }
}
