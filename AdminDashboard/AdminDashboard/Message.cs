using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdminDashboard.Request;

namespace AdminDashboard
{
    public class Message
    {
        private readonly HttpClient httpClient;

        public Message(string token)
        {
            this.httpClient = new HttpClient { BaseAddress = new Uri(Base.url(0)) };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<bool> CreateAsync(MessageRequest message)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(message);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"Meassages", content);
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
