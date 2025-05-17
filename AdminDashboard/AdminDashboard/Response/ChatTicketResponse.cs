using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminDashboard.Response
{
    public class ChatTicketResponse
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; }
        public int ChatId { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
