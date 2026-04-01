using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Contracts.Dtos.Users;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Storage;

namespace Booking.Infrastructure.Services;

public class UserService(InMemoryStore store) : IUserService
{
    public Task<UserProfileDto?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = store.Users.FirstOrDefault(x => x.Id == userId);
        return Task.FromResult(user?.ToDto());
    }

    public Task<IReadOnlyList<string>> GetFavoritesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var favorites = store.Users
            .FirstOrDefault(x => x.Id == userId)?
            .Favorites
            .ToList() ?? [];

        return Task.FromResult<IReadOnlyList<string>>(favorites);
    }

    public Task<bool> AddFavoriteAsync(string userId, string hotelId, CancellationToken cancellationToken = default)
    {
        var user = store.Users.FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            return Task.FromResult(false);
        }

        if (!user.Favorites.Contains(hotelId))
        {
            user.Favorites.Add(hotelId);
        }

        return Task.FromResult(true);
    }

    public Task<bool> RemoveFavoriteAsync(string userId, string hotelId, CancellationToken cancellationToken = default)
    {
        var user = store.Users.FirstOrDefault(x => x.Id == userId);
        if (user is null)
        {
            return Task.FromResult(false);
        }

        user.Favorites.Remove(hotelId);
        return Task.FromResult(true);
    }
}