using Booking.Domain.Hotels;
using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Persistence.Configurations;

public class HotelSubmissionConfiguration : IEntityTypeConfiguration<HotelSubmission>
{
    public void Configure(EntityTypeBuilder<HotelSubmission> builder)
    {
        builder.ToTable("hotel_submissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(64);

        builder.Property(x => x.SubmittedByUserId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.ApprovedHotelId)
            .HasMaxLength(64);

        builder.Property(x => x.Status)
            .HasMaxLength(50)
            .IsRequired();

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

        builder.Property(x => x.Rooms)
            .HasColumnType("jsonb");

        builder.Property(x => x.AdminComment)
            .HasMaxLength(2000);

        builder.Property(x => x.ReviewedByAdminId)
            .HasMaxLength(64);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(x => x.ReviewedByAdminId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(x => x.SubmissionType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.TargetHotelId)
            .HasMaxLength(64);

        builder.HasIndex(x => x.SubmissionType);
        builder.HasIndex(x => x.TargetHotelId);

        builder.HasIndex(x => x.SubmittedByUserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}