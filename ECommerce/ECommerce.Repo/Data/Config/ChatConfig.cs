using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using ECommerce.Core.Models;

namespace ECommerce.Repo.Data.Config
{
    public class ChatConfig : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasOne(x => x.Agent)
                .WithMany()
                .HasForeignKey(x => x.AgentId);

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId);

            builder.HasOne(x => x.Ticket)
                .WithOne(x => x.Chat)
                .HasForeignKey<ChatTicket>(x => x.ChatId)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion(O => O.ToString(), O => (StatusOptions)Enum.Parse(typeof(StatusOptions), O))
                .HasDefaultValue(StatusOptions.Pending);
        }
    }
}
