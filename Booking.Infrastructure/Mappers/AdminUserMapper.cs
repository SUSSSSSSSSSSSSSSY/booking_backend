using Booking.Contracts.Dtos.Admin;
using Booking.Domain.Users;

namespace Booking.Infrastructure.Mappers;

public static class AdminUserMapper
{
    public static AdminUserDto ToAdminDto(this AppUser user)
    {
        return new AdminUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            IsBlocked = user.IsBlocked,
            Verified = user.Verified,
            Phone = user.Phone ?? "",
            Country = user.Country ?? "",
            PreferredCurrency = user.PreferredCurrency,
            Birthday = user.Birthday?.ToString("yyyy-MM-dd") ?? "",
            FavoritesCount = user.Favorites?.Count ?? 0,
            CreatedAtUtc = user.CreatedAtUtc
        };
    }
}