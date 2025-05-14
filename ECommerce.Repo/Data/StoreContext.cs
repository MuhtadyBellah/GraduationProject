using System.Reflection;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Laravel;
using ECommerce.Core.Models.Order;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repo.Data
{
    public class StoreContext(DbContextOptions<StoreContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Product>())
            {
                if ((entry.State == EntityState.Added || entry.State == EntityState.Modified) && !string.IsNullOrEmpty(entry.Entity.PictureUrl) && !string.IsNullOrEmpty(entry.Entity.UrlGlb))
                {
                    entry.Entity.PictureUrl = $"Images/Products/{Path.GetFileName(entry.Entity.PictureUrl)}";
                    entry.Entity.UrlGlb = $"Images/Products/{Path.GetFileName(entry.Entity.UrlGlb)}";
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Favorites> Favorites { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatTicket> ChatTickets { get; set; }
    }
}
