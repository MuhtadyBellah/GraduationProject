using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace AdminDashboard
{
    public class Brand
    {
        private readonly HttpClient httpClient;

        public Brand(string token)
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
                var response = await httpClient.GetAsync($"Brands/count");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<int>(jsonResponse, new JsonSerializerOptions
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
        public async Task<IEnumerable<BrandResponse>> GetAllAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"Brands");
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<List<BrandResponse>>(jsonResponse, new JsonSerializerOptions
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
        public async Task<BrandResponse> GetByIdAsync(int id)
        {
            try
            {
                var response = await httpClient.GetAsync($"Brands/{id}");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<BrandResponse>(jsonResponse, new JsonSerializerOptions
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
        public async Task<bool> CreateAsync(BrandResponse entity)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("1"), "id");
            formData.Add(new StringContent(entity.Name), "name");
            formData.Add(new StringContent(entity.Description), "description");

            try
            {

                var response = await httpClient.PostAsync($"Brands", formData);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateAsync(int id, BrandResponse entity)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent("10"), "id");
            formData.Add(new StringContent(entity.Name), "name");
            formData.Add(new StringContent(entity.Description), "description");

            try
            {
                var response = await httpClient.PutAsync($"Brands/{id}", formData);
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

                var response = await httpClient.DeleteAsync($"Brands/{id}");
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
