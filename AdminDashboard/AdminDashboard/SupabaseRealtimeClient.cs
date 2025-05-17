using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminDashboard
{
    public class SupabaseRealtimeClient
    {
        private readonly ClientWebSocket _socket;
        private readonly string _apiKey;
        private readonly string _url;
        private RichTextBox chatBox;
        private readonly string _token;

        public SupabaseRealtimeClient(RichTextBox _chatBox, string token)
        {
            _token = token;
            chatBox = _chatBox;
            _socket = new ClientWebSocket();
            _url = "wss://bazvfoiiqfamubdjqgoi.supabase.co/realtime/v1";
            _apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJhenZmb2lpcWZhbXViZGpxZ29pIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzQwODk0MzAsImV4cCI6MjA0OTY2NTQzMH0.C1PexAyagnfqHT6IwnXOziLMzSYQbJ8kmmioeTl4C_k";
        }

        public async Task ConnectAsync()
        {
            // Set WebSocket headers
            _socket.Options.SetRequestHeader("apikey", _apiKey);
            _socket.Options.SetRequestHeader("Authorization", $"Bearer {_apiKey}");

            // Connect
            
            await _socket.ConnectAsync(new Uri($"{_url}?apikey={_apiKey}"), CancellationToken.None);

            // Subscribe to Messages table
            string joinPayload = @"{
                ""topic"":""realtime:public:Messages"",
                ""event"":""phx_join"",
                ""payload"":{},
                ""ref"":""1""
            }";

            await SendAsync(joinPayload);
            await ReceiveMessagesAsync(); // Run receive loop in background
        }
        public async Task DisconnectAsync()
        {
            if (_socket.State == WebSocketState.Open)
            {
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }

        /*PostMessage
        public async Task<bool> PostMessage(int chatId, string message)
        {
            var role = await GetRole();
            AppendChatMessage(message, role);
            var payload = new
            {
                chat_id = chatId,
                content = message,
                sender = role
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("apikey", _apiKey);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
                    var response = await client.PostAsync($"{_url}/rest/v1/Messages", content);
                    response.EnsureSuccessStatusCode();
                    return true;
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        */
        private async Task SendAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            await _socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[4096];
            while (_socket.State == WebSocketState.Open)
            {
                var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                try
                {
                    using (var doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;


                        if (root.TryGetProperty("event", out var evt) &&
                            evt.GetString() == "INSERT")
                        {
                            var msg = root.GetProperty("payload")
                                          .GetProperty("record")
                                          .GetProperty("content")
                                          .GetString();

                            var sender = root.GetProperty("payload")
                                             .GetProperty("record")
                                             .GetProperty("sender")
                                             .GetString();

                            AppendChatMessage(msg, sender);
                        }
                    }
                }
                catch { /* Handle or log malformed JSON */ }
            }
        }
        private void AppendChatMessage(string message, string sender)
        {
            if (chatBox.InvokeRequired)
            {
                chatBox.Invoke(new Action(() => AppendMessageToChat(message, sender)));
            }
            else
            {
                AppendMessageToChat(message, sender);
            }
        }

        private void AppendMessageToChat(string message, string sender)
        {
            // Check if the sender is "You" (user's message) or someone else
            if (sender == "user")
            {
                // Align to the right (for sent messages)
                chatBox.SelectionAlignment = HorizontalAlignment.Right;
                chatBox.SelectionColor = System.Drawing.Color.Blue; // Optional: color the message
            }
            else if (sender == "admin") // For admin messages
            {
                chatBox.SelectionAlignment = HorizontalAlignment.Left;
                chatBox.SelectionColor = System.Drawing.Color.Green; // Green for admin messages
            }

            // Append the message with the sender's name
            chatBox.AppendText($"{sender}: {message}\n");
        }

    }
}
