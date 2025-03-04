using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ECommerce.Core.Models.Laravel;

namespace ECommerce.Repo.Data.Config
{
    public class LaravelConfig : IEntityTypeConfiguration<Users>, IEntityTypeConfiguration<Orders>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.ToTable("users", t => t.ExcludeFromMigrations());
        }

        public void Configure(EntityTypeBuilder<Orders> builder)
        {
            builder.ToTable("orders", t => t.ExcludeFromMigrations());
        }
    }
}
