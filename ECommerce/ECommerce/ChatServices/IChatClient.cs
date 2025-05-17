using ECommerce.DTO.Response;

namespace ECommerce.ChatServices
{
    public interface IChatClient
    {
        public Task ReceiveUserMessages(string userDisplay, IEnumerable<MessageResponse> messages);
        public Task ReceiveMessage(string userDisplay, string message);
        public Task AskBot(string userDisplay, string message);
        public Task BroadCast(string system, string message);
        public Task ReceiveFile(string userDisplay, string file);
        public Task ReceiveAudio(string userDisplay, string audio);
    }
}
