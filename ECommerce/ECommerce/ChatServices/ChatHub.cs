using AutoMapper;
using ECommerce.Core;
using ECommerce.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ECommerce.ChatServices
{
    [Authorize("Sanctum")]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IUnitWork _unitWork;
        private readonly IMapper _mapper;
        private readonly GeminiService _gemini;

        public ChatHub(IUnitWork unitWork, IMapper mapper, GeminiService gemini)
        {
            _unitWork = unitWork;
            _mapper = mapper;
            _gemini = gemini;
        }

        public override async Task OnConnectedAsync()
        {
            var _contextHttp = Context.GetHttpContext();
            var chatId = _contextHttp?.Request.Query["chatId"];
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null || chatId is null) return;

            connMapping.Add(userId, Context.ConnectionId);
            await Clients.All.BroadCast("System", $"{Context.ConnectionId} connected");
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            await Clients.Group(chatId).BroadCast("System", $"{Context.ConnectionId} joined {chatId}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return;

            connMapping.TryRemove(userId);
            await Clients.All.BroadCast("System", $"User with ConnectionId: '{Context.ConnectionId}' disconnected");
        }

        public async Task SendMessage(string userDisplay, int chatId, string message)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return;

            var connectionId = connMapping.TryGetValue(userId);
            if (connectionId == null) return;

            var newMessage = new Message
            {
                ChatId = chatId,
                Content = message,
                IsRead = false,
                SentAt = DateTimeOffset.UtcNow,
                SenderId = int.Parse(userId),
                connectionId = connectionId
            };

            await _unitWork.Repo<Message>().AddAsync(newMessage);
            await _unitWork.CompleteAsync();

            if (message.StartsWith($"@bot"))
            {
                string botResponse =
                    await _gemini.AskGemini($"You are a helpful assistant in Customer Service, Billing, Call Center, etc. Question: {message.Replace("@bot", "")}");

                await Clients.Group(chatId.ToString()).AskBot("Bot", botResponse);
                return;
            }

            await Clients.Group(chatId.ToString()).ReceiveMessage(userDisplay, message);
        }

        // Allows a user to join a chat room
        public async Task JoinRoom(string userDisplay, int chatId)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return;

            var connectionId = connMapping.TryGetValue(userId);
            if (connectionId == null) return;

            await Groups.AddToGroupAsync(connectionId, chatId.ToString());
            await Clients.Group(chatId.ToString()).BroadCast("System", $"JoinRoom -> {userDisplay} joined {chatId}");
        }

        // Allows a user to leave a chat room
        public async Task LeaveRoom(string userDisplay, int chatId)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return;

            var connectionId = connMapping.TryGetValue(userId);
            if (connectionId == null) return;

            await Groups.RemoveFromGroupAsync(connectionId, chatId.ToString());
            await Clients.Group(chatId.ToString()).BroadCast("System", $"LeaveRoom -> {userDisplay} left {chatId}");
        }

        public async Task SendFile(string userDisplay, int chatId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                await Clients.All.BroadCast("System", $"SendFile -> Invalid File");
                return;
            }

            var fileName = Path.GetFileName(file?.FileName);
            //var filePath = Path.Combine("wwwroot/uploads/file", fileName);
            //Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            //using (var stream = new FileStream(filePath, FileMode.Create))
            //    await file.CopyToAsync(stream);

            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return;

            var connectionId = connMapping.TryGetValue(userId);
            if (connectionId == null) return;

            var newMessage = new Message
            {
                ChatId = chatId,
                Content = fileName,
                IsRead = false,
                SentAt = DateTimeOffset.UtcNow,
                SenderId = int.Parse(userId),
                connectionId = connectionId
            };

            await _unitWork.Repo<Message>().AddAsync(newMessage);
            await _unitWork.CompleteAsync();
            await Clients.Group(chatId.ToString()).ReceiveMessage(userDisplay, fileName);

            //await Clients.Client(connectionId).ReceiveFile(userDisplay, fileName);
            //await Clients.User(receiverId).ReceiveFile(userDisplay, fileName);
            //await Clients.Group(roomName).ReceiveFile(userDisplay, fileName);
        }

        public async Task SendAudio(string userDisplay, int chatId, IFormFile audio)
        {
            if (audio == null || audio.Length == 0)
                await Clients.All.BroadCast("System", $"SendAudio -> Invalid File");

            var fileName = Path.GetFileName(audio?.FileName);
            //var audioPath = Path.Combine("wwwroot/uploads/audio", audioFileName);
            //Directory.CreateDirectory(Path.GetDirectoryName(audioPath));
            //using (var stream = new FileStream(audioPath, FileMode.Create))
            //    await audio.CopyToAsync(stream);

            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return;

            var connectionId = connMapping.TryGetValue(userId);
            if (connectionId == null) return;

            var newMessage = new Message
            {
                ChatId = chatId,
                Content = fileName,
                IsRead = false,
                SentAt = DateTimeOffset.UtcNow,
                SenderId = int.Parse(userId),
                connectionId = connectionId
            };

            await _unitWork.Repo<Message>().AddAsync(newMessage);
            await _unitWork.CompleteAsync();
            await Clients.Group(chatId.ToString()).ReceiveMessage(userDisplay, fileName);

            //await Clients.Client(connectionId).ReceiveFile(userDisplay, fileName);
            //await Clients.User(receiverId).ReceiveFile(userDisplay, fileName);
            //await Clients.Group(roomName).ReceiveFile(userDisplay, fileName);
        }
    }
}
