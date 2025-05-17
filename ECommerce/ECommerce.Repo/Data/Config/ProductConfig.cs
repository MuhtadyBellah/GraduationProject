using ECommerce.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ECommerce.Repo.Data.Config
{
    public class ProductConfig : IEntityTypeConfiguration<Product>, IEntityTypeConfiguration<ProductBrand>, IEntityTypeConfiguration<ProductType>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(p => p.ProductBrand)
                .WithMany()
                .HasForeignKey(p => p.ProductBrandId);

            builder.HasOne(p => p.ProductType)
                .WithMany()
                .HasForeignKey(p => p.ProductTypeId);


            builder.Property(p => p.Name).IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.PictureUrl)
                .IsRequired();

            builder.Property(p => p.UrlGlb)
                .IsRequired();

            builder.Property(p => p.Price).IsRequired()
                .HasColumnType("decimal(18,2)");
        }

        public void Configure(EntityTypeBuilder<ProductBrand> builder)
            => builder.Property(p => p.Name).IsRequired();

        public void Configure(EntityTypeBuilder<ProductType> builder)
            => builder.Property(p => p.Name).IsRequired();
    }
}
