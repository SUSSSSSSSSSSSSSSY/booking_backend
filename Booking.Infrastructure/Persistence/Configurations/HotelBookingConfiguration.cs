using Booking.Domain.Bookings;
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

        builder.HasOne<Booking.Domain.Users.AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Booking.Domain.Hotels.Hotel>()
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Booking.Domain.Hotels.Room>()
            .WithMany()
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}