using ECommerce.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Repo.Identity
{
    public static class UserContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> manager)
        {
            if (!manager.Users.Any())
            {
                var user = new AppUser()
                {
                    Name = "Muhtady",
                    Email = "muhybellah@gmail.com",
                    UserName = "muhybellah",
                    PhoneNumber = "1234567890",
                    Address = new Address()
                    {
                        FirstName = "Muhtady",
                        LastName = "Mahdy",
                        Street = "123 Main St",
                        City = "Eygpt",
                        Country = "Cairo"
                    }
                };
                await manager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}
