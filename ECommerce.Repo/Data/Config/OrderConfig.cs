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
    public class OrderConfig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.Status)
                .HasDefaultValue(OrderStatus.Pending)
                .HasConversion(O => O.ToString(), O => (OrderStatus) Enum.Parse(typeof(OrderStatus), O));
        
            builder.Property(x => x.SubTotal)
                .HasColumnType("decimal(18,2)");

            builder.OwnsOne(O => O.ShippingAddress, SA => SA.WithOwner());

            builder.HasOne(builder => builder.Delivery)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        } 
    }
}
