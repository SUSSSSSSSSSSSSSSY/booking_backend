using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Infrastructure.Persistence.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(64);

        builder.Property(x => x.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(1000);

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        builder.Property(x => x.Country)
            .HasMaxLength(100);

        builder.Property(x => x.PreferredCurrency)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.GoogleSubjectId)
            .HasMaxLength(200);

        builder.HasIndex(x => x.GoogleSubjectId)
            .IsUnique();

        builder.Property(x => x.PictureUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.Favorites)
            .HasColumnType("jsonb");
    }
}