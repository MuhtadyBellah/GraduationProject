using AdminDashboard.Request;
using AdminDashboard.Request.AdminDashboard.Request;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminDashboard
{
    public class Coupon
    {
        private readonly HttpClient httpClient;
        
        public Coupon(string token)
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
                var response = await httpClient.GetAsync($"admin/coupons/index");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<CouponResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return res.coupons.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        public async Task<CouponResponse> GetAllAsync()
        {
            try
            {
                var response = await httpClient.GetAsync($"admin/coupons/index");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var res = JsonSerializer.Deserialize<CouponResponse>(jsonResponse, new JsonSerializerOptions
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
                var response = await httpClient.DeleteAsync($"admin/coupons/delete/{id}");
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
