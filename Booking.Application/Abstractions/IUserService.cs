using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Contracts.Dtos.Users;

namespace Booking.Application.Abstractions;

public interface IUserService
{
    Task<UserProfileDto?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetFavoritesAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> AddFavoriteAsync(string userId, string hotelId, CancellationToken cancellationToken = default);
    Task<bool> RemoveFavoriteAsync(string userId, string hotelId, CancellationToken cancellationToken = default);
}