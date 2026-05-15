using Booking.Domain.Bookings;
using Booking.Domain.Hotels;
using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Persistence.Configurations;

public class HotelBookingConfiguration : IEntityTypeConfiguration<HotelBooking>
{
    public void Configure(EntityTypeBuilder<HotelBooking> builder)
    {
        builder.ToTable("bookings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(64);

        builder.Property(x => x.UserId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.HotelId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.RoomId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TotalPrice)
            .HasColumnType("numeric(10,2)");

        builder.Property(x => x.Currency)
            .HasMaxLength(10)
            .IsRequired();

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.HotelOwnerUserId)
            .HasMaxLength(64);

        builder.Property(x => x.IsHiddenForUser)
            .IsRequired();

        builder.Property(x => x.CancellationReason)
            .HasMaxLength(1000);

        builder.Property(x => x.OwnerRespondedAtUtc);

        builder.Property(x => x.CancelledAtUtc);

        builder.HasIndex(x => x.HotelOwnerUserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.RoomId, x.CheckIn, x.CheckOut });

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.HotelId);
        builder.HasIndex(x => x.RoomId);
        builder.HasIndex(x => x.Status);
    }
}