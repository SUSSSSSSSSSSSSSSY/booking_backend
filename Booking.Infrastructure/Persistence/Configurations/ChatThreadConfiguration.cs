using Booking.Domain.Chats;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Persistence.Configurations;

public class ChatThreadConfiguration : IEntityTypeConfiguration<ChatThread>
{
    public void Configure(EntityTypeBuilder<ChatThread> builder)
    {
        builder.ToTable("chat_threads");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(64);

        builder.Property(x => x.BookingId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.GuestUserId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.OwnerUserId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.HotelId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.RoomId)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasMany(x => x.Messages)
            .WithOne(x => x.Thread)
            .HasForeignKey(x => x.ThreadId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.BookingId)
            .IsUnique();

        builder.HasIndex(x => x.GuestUserId);
        builder.HasIndex(x => x.OwnerUserId);
        builder.HasIndex(x => x.HotelId);
        builder.HasIndex(x => x.LastMessageAtUtc);
    }
}