using ECommerce.Core.Models;

namespace ECommerce.Core.Specifications
{
    public class MessageSpec : BaseSpecification<Message>
    {
        public MessageSpec(int userId, string s = "user") : base(p => p.SenderId == userId)
        {
            Includes.Add(p => p.User);
            Includes.Add(p => p.Chat);
        }
        public MessageSpec(int id) : base(p => p.Id == id) { }
        public MessageSpec(List<int> ids) : base(p => ids.Contains(p.Id)) { }
    }
}
