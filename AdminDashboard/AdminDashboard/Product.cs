using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using AdminDashboard.Request;

namespace AdminDashboard
{
    public class Product
    {
        private readonly HttpClient httpClient;

        public Product(string token)
        {
            this.httpClient = new HttpClient { BaseAddress = new Uri(Base.url(0))};
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<int?> GetTotalCountAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"Product");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<ProductPaged>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return res.count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        public async Task<ProductPaged> GetAllPagedAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"Product");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<ProductPaged>(jsonResponse, new JsonSerializerOptions
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
        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"Product");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<List<ProductResponse>>(jsonResponse, new JsonSerializerOptions
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
        public async Task<ProductResponse> GetByIdAsync(int id)
        {
            try
            {
                var response = await httpClient.GetAsync($"Product/{id}");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<ProductResponse>(jsonResponse, new JsonSerializerOptions
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
        public async Task<bool> CreateAsync(ProductRequest product)
        {
            // Create multipart form data
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(product.Name), "Name");
            formData.Add(new StringContent(product.Description), "Description");
            formData.Add(new StringContent(product.Price.ToString()), "Price");
            formData.Add(new StringContent(product.productBrandId.ToString()), "ProductBrandId");
            formData.Add(new StringContent(product.productTypeId.ToString()), "ProductTypeId");
            formData.Add(new StringContent(product.quantity.ToString()), "Quantity");

            // Add image files if they exist
            if (product.PictureFile != null)
            {
                var streamContent = new StreamContent(product.PictureFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // or PNG
                formData.Add(streamContent, "PictureFile", "image1.jpg");
            }

            if (product.PictureFileGlB != null)
            {
                var streamContent = new StreamContent(product.PictureFileGlB.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                formData.Add(streamContent, "PictureFileGlB", "image2.jpg");
            }

            try
            {
                var response = await httpClient.PostAsync($"Product", formData);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateAsync(int id, ProductRequest entity)
        {
            // Create multipart form data
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(entity.Name), "Name");
            formData.Add(new StringContent(entity.Description), "Description");
            formData.Add(new StringContent(entity.Price.ToString()), "Price");
            formData.Add(new StringContent(entity.productBrandId.ToString()), "ProductBrandId");
            formData.Add(new StringContent(entity.productTypeId.ToString()), "ProductTypeId");
            formData.Add(new StringContent(entity.quantity.ToString()), "Quantity");

            try
            {
                var response = await httpClient.PutAsync($"Product/{id}", formData);
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
                var response = await httpClient.DeleteAsync($"Product/{id}");
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
