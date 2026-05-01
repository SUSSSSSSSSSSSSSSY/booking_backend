using Booking.Domain.Bookings;
using Booking.Domain.Hotels;
using Booking.Domain.Reviews;
using Booking.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public class BookingDbContext(DbContextOptions<BookingDbContext> options)
    : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<UserRefreshToken> RefreshTokens => Set<UserRefreshToken>();

    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<HotelBooking> Bookings => Set<HotelBooking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingDbContext).Assembly);
    }
}