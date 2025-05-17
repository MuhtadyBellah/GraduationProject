using ECommerce.Core.Models.Order;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Models;

namespace ECommerce.Repo.Data.Config
{
    public class MessageConfig : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasOne(x => x.User)
               .WithMany(x => x.Messages)
               .HasForeignKey(x => x.SenderId)
               .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Chat)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ChatId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.Content)
                .IsRequired();

            builder.Property(x => x.IsRead)
                .HasDefaultValue(false);
        }
    }
}
