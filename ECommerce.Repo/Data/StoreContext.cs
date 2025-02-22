using System.Reflection;
using ECommerce.Core.Models;
using ECommerce.Core.Models.Order;
using ECommerce.Core.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Repo.Data
{
    public class StoreContext(DbContextOptions<StoreContext> options) : DbContext(options) 
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    foreach (var entry in ChangeTracker.Entries<Product>())
        //    {
        //        if (entry.State == EntityState.Added && string.IsNullOrEmpty(entry.Entity.PictureUrl))
        //        {
        //            entry.Entity.PictureUrl = $"https://dhqjcyiisuhpbzshaxnm.supabase.co/storage/v1/object/public/Images/Products/{entry.Entity.PictureUrl}";
        //        }
        //    }

        //    return await base.SaveChangesAsync(cancellationToken);
        //}

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderItem> orderItems { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}
