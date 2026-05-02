using Booking.Application.Abstractions;
using Booking.Contracts.Dtos.Users;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef;

public class EfUserService(BookingDbContext dbContext) : IUserService
{
    public async Task<UserProfileDto?> GetByIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        return user?.ToDto();
    }

    public async Task<IReadOnlyList<string>> GetFavoritesAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        return user?.Favorites?.ToList() ?? [];
    }

    public async Task<bool> AddFavoriteAsync(
        string userId,
        string hotelId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        var hotelExists = await dbContext.Hotels
            .AnyAsync(x => x.Id == hotelId && !x.IsDeleted, cancellationToken);

        if (!hotelExists)
        {
            return false;
        }

        user.Favorites ??= [];

        if (!user.Favorites.Contains(hotelId))
        {
            user.Favorites.Add(hotelId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RemoveFavoriteAsync(
        string userId,
        string hotelId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            return false;
        }

        user.Favorites ??= [];

        user.Favorites.Remove(hotelId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}