//using ECommerce.Core.Models.Identity;
//using ECommerce.Core.Services;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Configuration;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace ECommerce.Service
//{
//    public class TokenService : IToken
//    {
//        private readonly IConfiguration _configuration;
//        public TokenService(IConfiguration configuration)
//            => _configuration = configuration;
//        public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> manager)
//        {
//            //Payload - Private Claims [User - Defined]
//            if (user == null) throw new ArgumentNullException(nameof(user));
//            var auth = new List<Claim>()
//            {
//                new Claim(ClaimTypes.GivenName, user.FName ?? string.Empty),
//                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
//            };

//            var roles = await manager.GetRolesAsync(user);
//            foreach (var role in roles)
//                auth.Add(new Claim(ClaimTypes.Role, role));

//            try
//            {
//                var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
//                var token = new JwtSecurityToken(
//                    issuer: _configuration["JWT:Issuer"],
//                    audience: _configuration["JWT:Audience"],
//                    expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:Duration"])),
//                    claims: auth,
//                    signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
//                );
//                return new JwtSecurityTokenHandler().WriteToken(token);
//            }
//            catch (Exception ex)
//            {
//                // Log or inspect the exception
//                throw new InvalidOperationException("Error while creating JWT token", ex);
//            }
//        }
//    }
//}
