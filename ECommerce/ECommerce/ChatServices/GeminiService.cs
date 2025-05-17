using System.Text;
using System.Text.Json;

namespace ECommerce.ChatServices
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public GeminiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Gemini:key"];
            _apiUrl = config["Gemini:url"];
        }

        public async Task<string> AskGemini(string message, string? img = "", string? exten = "")
        {
            var parts = new List<object> { new { text = message } };

            if (!string.IsNullOrEmpty(img))
                parts.Add(
                    new { inlineData = new { mimeType = $"image/{exten}", data = img } } 
                    );

            var requestBody = new
            {
                contents = new[]
                {
                    new {
                        parts = parts.ToArray(),
                        role = "user"
                    }
                }
            };

            var request = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync
            (
                _apiUrl + $"?key={_apiKey}",
                request
            );
            if (!response.IsSuccessStatusCode) return "Sorry, something went wrong.";

            var result = await response.Content.ReadAsStringAsync();

            var json = JsonDocument.Parse(result);
            return
                json.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? "Sorry, I didn’t understand.";
        }
    }

}
