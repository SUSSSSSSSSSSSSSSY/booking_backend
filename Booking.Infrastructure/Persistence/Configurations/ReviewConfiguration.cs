using Booking.Domain.Hotels;
using Booking.Domain.Reviews;
using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(64);

        builder.Property(x => x.HotelId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(x => x.Author)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Text)
            .HasMaxLength(4000)
            .IsRequired();

        builder.HasOne<Hotel>()
            .WithMany()
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.HotelId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.IsDeleted);
    }
}