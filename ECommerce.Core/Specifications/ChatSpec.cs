using ECommerce.Core.Models;

namespace ECommerce.Core.Specifications
{
    public class ChatSpec : BaseSpecification<Chat>
    {
        public ChatSpec(StatusOptions option) : base(p => p.Status == option && p.AgentId == null) { }
        public ChatSpec(int agentId, int customerId) : base(p => p.CustomerId == customerId && p.AgentId == agentId) { }
        public ChatSpec(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.Messages);
            Includes.Add(p => p.Agent);
            Includes.Add(p => p.Customer);
            Includes.Add(p => p.Category);
            Includes.Add(p => p.Ticket);
        }
        public ChatSpec(int customerId, string s = "customer") : base(p => p.CustomerId == customerId)
        {
            Includes.Add(p => p.Messages);
            Includes.Add(p => p.Agent);
            Includes.Add(p => p.Customer);
            Includes.Add(p => p.Category);
            Includes.Add(p => p.Ticket);
        }
        public ChatSpec(List<int> ids) : base(p => ids.Contains(p.Id)) { }
    }
}
