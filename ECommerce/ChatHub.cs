using ECommerce.Core;
using ECommerce.Core.Models;
using ECommerce.Service.ChatHub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NuGet.Protocol.Plugins;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace ECommerce
{
    [Authorize]
    public sealed class ChatHub : Hub<IChatClient>
    {
        private static ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();
        private readonly IUnitWork _unitWork;

        public ChatHub(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) 
                return;

            _users[userId] = Context.ConnectionId;
            Console.WriteLine($"OnConnected: {userId} with {Context.UserIdentifier} connected");
            await Clients.All.BroadCast("System", $"{userId} :: {Context.UserIdentifier} with {Context.ConnectionId} connected");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            Console.WriteLine($"OnDisconnected: {userId} with {Context.UserIdentifier} disconnected");
            if (!string.IsNullOrEmpty(userId))
            {
                _users.TryRemove(userId, out _);
            }
            await Clients.All.BroadCast("System", $"{userId} with {Context.UserIdentifier} disconnected");
        }

        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine($"SendMessage: {Context.UserIdentifier} -> {user}: {message}");
            if (_users.TryGetValue(user, out string? connectionId))
            {
                await Clients.Client(connectionId).ReceiveMessage(Context.UserIdentifier, message);
            }
            else
            {
                Console.WriteLine($"SendMessage: Receiver {user} not found");
            }
        }

        // Handles file upload and sends the file URL to the receiver
        public async Task SendFile(string user, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                Console.WriteLine("SendFile: Invalid file received");
                return;
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine("wwwroot/uploads/file", fileName);
            Console.WriteLine($"SendFile: Saving file {fileName} at {filePath}");

            // Ensure directory exists before saving the file
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (_users.TryGetValue(user, out string? connectionId))
            {
                Console.WriteLine($"SendFile: Sending {fileName} to {user}");
                await Clients.Client(connectionId).ReceiveFile(Context.UserIdentifier, $"/uploads/{fileName}");
            }
            else
            {
                Console.WriteLine($"SendFile: Receiver {user} not found");
            }
        }

        // Handles audio recording upload and sends the audio file URL to the receiver
        public async Task SendAudio(string user, IFormFile audio)
        {
            if (audio == null || audio.Length == 0)
            {
                Console.WriteLine("SendAudio: Invalid audio received");
                return;
            }

            var audioFileName = Path.GetFileName(audio.FileName);
            var audioPath = Path.Combine("wwwroot/uploads/audio", audioFileName);
            Console.WriteLine($"SendAudio: Saving audio {audioFileName} at {audioPath}");

            // Ensure directory exists before saving the audio
            Directory.CreateDirectory(Path.GetDirectoryName(audioPath));

            using (var stream = new FileStream(audioPath, FileMode.Create))
            {
                await audio.CopyToAsync(stream);
            }

            if (_users.TryGetValue(user, out string? connectionId))
            {
                Console.WriteLine($"SendAudio: Sending {audioFileName} to {user}");
                await Clients.Client(connectionId).ReceiveAudio(Context.UserIdentifier, $"/uploads/audio/{audioFileName}");
            }
            else
            {
                Console.WriteLine($"SendAudio: Receiver {user} not found");
            }
        }

        // Allows a user to join a chat room
        public async Task JoinRoom(string roomName)
        {
            Console.WriteLine($"JoinRoom: {Context.UserIdentifier} joined {roomName}");
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).BroadCast("System", $"{Context.UserIdentifier} has joined {roomName}");
        }

        // Allows a user to leave a chat room
        public async Task LeaveRoom(string roomName)
        {
            Console.WriteLine($"LeaveRoom: {Context.UserIdentifier} left {roomName}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await Clients.Group(roomName).BroadCast("System", $"{Context.UserIdentifier} has left {roomName}");
        }

        public async Task SendMessageToGroup(string roomName, string message)
        {
            Console.WriteLine($"SendMessageToGroup: {Context.UserIdentifier} -> {roomName}: {message}");
            await Clients.Group(roomName).ReceiveMessage(Context.UserIdentifier, message);
        }
    }
}
