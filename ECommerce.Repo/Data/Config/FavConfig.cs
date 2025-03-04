using ECommerce.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Repo.Data.Config
{
    public class FavConfig: IEntityTypeConfiguration<Favorites>
    {
        public void Configure(EntityTypeBuilder<Favorites> builder){
            builder.HasOne(p => p.Product)
                .WithMany(p => p.Favorites)
                .HasForeignKey(p => p.ProductId);

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);
        }
    }
    
}
