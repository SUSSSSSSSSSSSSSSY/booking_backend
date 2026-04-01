using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Contracts.Requests.Auth;
using Booking.Contracts.Responses.Auth;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Storage;

namespace Booking.Infrastructure.Services;

public class AuthService(InMemoryStore store) : IAuthService
{
    public Task<AuthResponseDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = store.Users.FirstOrDefault(x =>
            x.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) &&
            x.Password == request.Password);

        if (user is null)
        {
            return Task.FromResult<AuthResponseDto?>(null);
        }

        const string accessToken = "token_access_demo";
        const string refreshToken = "token_refresh_demo";

        var response = new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = user.ToDto()
        };

        return Task.FromResult<AuthResponseDto?>(response);
    }
}