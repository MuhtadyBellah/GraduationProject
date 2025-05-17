using AdminDashboard.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdminDashboard.Response;

namespace AdminDashboard
{
    public class Chat
    {
        private readonly HttpClient httpClient;

        public Chat(string token)
        {
            this.httpClient = new HttpClient { BaseAddress = new Uri(Base.url(0)) };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<ChatResponse> CreateAsync(string category = "Other")
        {
            try
            {
                var response = await httpClient.PostAsync($"Chats/{category}", null);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<ChatResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> UpdateAsync(int chatId)
        {
            try
            {
                var response = await httpClient.PutAsync($"Chats/{chatId}", null);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
