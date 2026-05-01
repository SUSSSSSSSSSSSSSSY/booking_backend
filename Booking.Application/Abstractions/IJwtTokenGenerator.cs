using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Domain.Users;

namespace Booking.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(AppUser user);
    string GenerateRefreshToken();
    string HashRefreshToken(string refreshToken);
}