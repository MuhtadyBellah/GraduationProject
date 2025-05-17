using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using AdminDashboard.Request;
using Newtonsoft.Json.Linq;

namespace AdminDashboard
{
    public class Profile 
    {
        private readonly HttpClient httpClient;
        
        public Profile(string token)
        {
            this.httpClient = new HttpClient{ BaseAddress = new Uri(Base.url(1)) };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public async Task<int?> GetTotalCountAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"admin/users/index");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<UserResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return res.users.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        public async Task<string> GetRole()
        {
            try
            {
                var response = await httpClient.GetAsync($"profile");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<ProfileResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return res.User.Role;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<UserResponse> GetAllUsersAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"admin/users/index");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<UserResponse>(jsonResponse, new JsonSerializerOptions
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

        public async Task<ProfileResponse> GetUserAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"profile");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<ProfileResponse>(jsonResponse, new JsonSerializerOptions
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
        public async Task<bool> UpdateAsync(int id, string role)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(role), "role");

            try
            {
                var response = await httpClient.PutAsync($"admin/users/update/{id}", formData);
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
                var response = await httpClient.DeleteAsync($"admin/users/delete/{id}");
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
