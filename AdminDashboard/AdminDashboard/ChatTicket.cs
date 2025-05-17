using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AdminDashboard.Request;
using AdminDashboard.Response;

namespace AdminDashboard
{
    public class ChatTicket
    {
        private readonly HttpClient httpClient;

        public ChatTicket(string token)
        {
            this.httpClient = new HttpClient { BaseAddress = new Uri(Base.url(0)) };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<int?> GetTotalCountAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"ChatTickets");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<List<ChatTicketResponse>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return res.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        public async Task<IEnumerable<ChatTicketResponse>> GetAllAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"ChatTickets");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<List<ChatTicketResponse>>(jsonResponse, new JsonSerializerOptions
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
        public async Task<ChatTicketResponse> GetByIdAsync(int id)
        {
            try
            {
                var response = await httpClient.GetAsync($"/ChatTickets/{id}");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<ChatTicketResponse>(jsonResponse, new JsonSerializerOptions
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
        public async Task<bool> CreateAsync(ChatTicketRequest entity)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(entity);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"ChatTickets", content);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"ChatTickets/{id}");
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
