using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminDashboard
{
    public class Login
    {
        private readonly HttpClient httpClient;
        private readonly string email;
        private readonly string password;

        public Login(string email, string password)
        {
            this.email = email;
            this.password = password;
            this.httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Base.url(1));
            httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<ProfileResponse> AuthenticateUserAsync()
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(email), "email");
            formData.Add(new StringContent(password), "password");

            //var rawData = new
            //{
            //    email = email,
            //    password = password
            //};

            //var jsonContent = JsonSerializer.Serialize(requestData);
            //var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {   
                var response = await httpClient.PostAsync("login", formData);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var profileResponse = JsonSerializer.Deserialize<ProfileResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return profileResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
