using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Policy;
using AdminDashboard.Request;
using AdminDashboard.Request.AdminDashboard.Request;

namespace AdminDashboard
{
    public class Order
    {
        private readonly HttpClient httpClient;
        
        public Order(string token)
        {
            httpClient = new HttpClient { BaseAddress = new Uri(Base.url(1)) };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<int?> GetTotalCountAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"admin/orders/index");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<OrderResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return res.orders.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        public async Task<OrderResponse> GetAllAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"admin/orders/index");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<OrderResponse>(jsonResponse, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching orders: {ex.Message}");
                return null;
            }
        }
        public async Task<bool> CreateAsync(CouponRequest coupon)
        {
            // Create multipart form data
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(coupon.name), "name");
            formData.Add(new StringContent(coupon.duration), "duration");
            formData.Add(new StringContent(coupon.percent_off), "percent_off");
            formData.Add(new StringContent(coupon.coupon_number), "coupon_number");

            try
            {
                var response = await httpClient.PostAsync($"admin/coupons/store", formData);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateAsync(int id, CouponRequest coupon)
        {
            // Create multipart form data
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(coupon.name), "name");
            formData.Add(new StringContent(coupon.duration), "duration");
            formData.Add(new StringContent(coupon.percent_off), "percent_off");

            try
            {
                var response = await httpClient.PutAsync($"admin/coupons/update/{id}", formData);
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
                var response = await httpClient.DeleteAsync($"admin/delete/orders/index/{id}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching orders: {ex.Message}");
                return false;
            }
        }
    }
}
