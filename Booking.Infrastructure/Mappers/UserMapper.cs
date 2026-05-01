using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Contracts.Dtos.Users;
using Booking.Domain.Users;

namespace Booking.Infrastructure.Mappers;

public static class UserMapper
{
    public static UserProfileDto ToDto(this AppUser user)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Verified = user.Verified,
            Phone = user.Phone ?? "",
            Country = user.Country ?? "",
            PreferredCurrency = user.PreferredCurrency,
            Birthday = user.Birthday?.ToString("yyyy-MM-dd") ?? "",
            Favorites = user.Favorites?.ToList() ?? []
        };
    }
}