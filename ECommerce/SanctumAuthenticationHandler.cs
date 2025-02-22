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
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userInfo = await ValidateSanctumToken(token);

            if (userInfo == null)
            {
                return AuthenticateResult.Fail("Invalid Sanctum Token");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userInfo.Name),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Sanctum");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Sanctum");

            return AuthenticateResult.Success(ticket);
        }

        private async Task<UserInfo?> ValidateSanctumToken(string token)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync("https://concise-ant-sound.ngrok-free.app/api/user");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserInfo>(jsonResponse);
        }

        private class UserInfo
        {
            public required string Email { get; set; }
            public required string Name { get; set; }
            public required int Id { get; set; }
        }
    }
}