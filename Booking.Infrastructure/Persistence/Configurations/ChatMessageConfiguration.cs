using Booking.Domain.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Persistence.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("chat_messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(64);

        builder.Property(x => x.ThreadId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.SenderUserId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.Text)
            .HasMaxLength(4000)
            .IsRequired();

        builder.HasIndex(x => x.ThreadId);
        builder.HasIndex(x => x.SenderUserId);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}