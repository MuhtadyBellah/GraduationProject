using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ECommerce.Core.Models;

namespace ECommerce.Repo.Data.Config
{
    internal class TicketConfig : IEntityTypeConfiguration<ChatTicket>
    {
        public void Configure(EntityTypeBuilder<ChatTicket> builder)
        {
            builder.HasIndex(x => x.ChatId)
                .IsUnique();

            builder.HasOne(x => x.Chat)
                .WithOne(x => x.Ticket)
                .HasForeignKey<ChatTicket>(x => x.ChatId)
                .IsRequired();
        }
    }
}
