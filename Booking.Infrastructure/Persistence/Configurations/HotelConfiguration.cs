using Booking.Domain.Hotels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Persistence.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.ToTable("hotels");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(64);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Country)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.PricePerNight)
            .HasColumnType("numeric(10,2)");

        builder.Property(x => x.DistanceToCenterKm)
            .HasColumnType("numeric(10,2)");

        builder.Property(x => x.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(x => x.Tags)
            .HasColumnType("jsonb");

        builder.Property(x => x.Amenities)
            .HasColumnType("jsonb");

        builder.Property(x => x.Images)
            .HasColumnType("jsonb");

        builder.Property(x => x.ScoreItems)
            .HasColumnType("jsonb");

        builder.Property(x => x.Facilities)
            .HasColumnType("jsonb");

        builder.HasMany(x => x.Rooms)
            .WithOne()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.OwnerUserId)
            .HasMaxLength(64);

        builder.Property(x => x.IsUserSubmitted)
            .IsRequired();

        builder.HasIndex(x => x.OwnerUserId);

        builder.HasIndex(x => x.City);
        builder.HasIndex(x => x.Country);
        builder.HasIndex(x => x.IsDeleted);
    }
}