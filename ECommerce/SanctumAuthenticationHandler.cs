using ECommerce.Helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ECommerce
{
    public class SanctumAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public SanctumAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var userInfo = await ValidateSanctumToken(token);
                if (userInfo == null)
                    return AuthenticateResult.Fail("Invalid Sanctum Token");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userInfo.Name),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                    new Claim(ClaimTypes.Role, userInfo.Role),
                };

                var schemeName = userInfo.Role.Equals("user", StringComparison.OrdinalIgnoreCase) ? "Sanctum" : "Admin";
                var identity = new ClaimsIdentity(claims, schemeName);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, schemeName);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }
        }

        private async Task<UserInfo?> ValidateSanctumToken(string token)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync("https://concise-ant-sound.ngrok-free.app/api/profile");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var profileResponse = JsonSerializer.Deserialize<ProfileResponse>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return profileResponse?.User;
        }

        private class ProfileResponse
        {
            public required UserInfo User { get; set; }
        }

        private class UserInfo
        {
            public required int Id { get; set; }
            public required string Name { get; set; }
            public required string Email { get; set; }
            public required string Role { get; set; }
        }
    }
}