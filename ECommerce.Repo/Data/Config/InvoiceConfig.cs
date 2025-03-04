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
    public class InvoiceConfig : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.User)
                .WithMany(x => x.Invoices)
                .HasForeignKey(x => x.userId);

            builder.HasOne(x => x.Order)
                .WithOne(x => x.Invoice)
                .HasForeignKey<Invoice>(x => x.orderId);

            builder.Property(x => x.InvoiceDate).HasColumnType("timestamp without time zone");
            builder.Property(x => x.PaymentDate).HasColumnType("timestamp without time zone");
        }
    }
}
