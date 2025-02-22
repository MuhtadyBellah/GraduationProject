using ECommerce.Core.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repo.Identity
{
    public class UserContext : IdentityDbContext<AppUser>
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }
        public DbSet<AppUser> Users { get; set; }
    }
}
