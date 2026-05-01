using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Contracts.Requests.Auth;
using Booking.Contracts.Responses.Auth;

namespace Booking.Application.Abstractions;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponseDto> LoginWithGoogleAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponseDto?> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
}