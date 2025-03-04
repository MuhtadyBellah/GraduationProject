using ECommerce.Core.Models.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repo.Data.Config
{
    public class DeliveryConfig : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> builder)
        {
            builder.Property(x => x.Cost).HasColumnType("decimal(18,2)");
            
            builder.Property(x => x.Status)
                .HasDefaultValue(DeliveryStatus.Pending)
                .HasConversion(O => O.ToString(), O => (DeliveryStatus) Enum.Parse(typeof(DeliveryStatus), O));

            builder.HasMany(x => x.Orders)
                .WithOne(x => x.Delivery)
                .HasForeignKey(x => x.deliveryId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}