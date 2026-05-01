using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Contracts.Requests.Auth;
using Booking.Contracts.Responses.Auth;
using Booking.Domain.Users;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Storage;
using Microsoft.Extensions.Options;
using Booking.Infrastructure.Auth;

namespace Booking.Infrastructure.Services;

public class AuthService(
    InMemoryStore store,
    IPasswordService passwordService,
    IJwtTokenGenerator jwtTokenGenerator,
    IGoogleAuthService googleAuthService,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public Task<AuthResponseDto?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = store.Users.FirstOrDefault(x =>
            x.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (user is null)
        {
            return Task.FromResult<AuthResponseDto?>(null);
        }

        if (user.IsBlocked)
        {
            return Task.FromResult<AuthResponseDto?>(null);
        }

        var passwordIsValid = passwordService.VerifyPassword(user, request.Password);
        if (!passwordIsValid)
        {
            return Task.FromResult<AuthResponseDto?>(null);
        }

        var response = CreateAuthResponse(user);
        return Task.FromResult<AuthResponseDto?>(response);
    }

    public Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = store.Users.FirstOrDefault(x =>
            x.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (existingUser is not null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = new AppUser
        {
            Id = $"usr_{Guid.NewGuid():N}"[..12],
            FullName = request.FullName,
            Email = request.Email.Trim().ToLowerInvariant(),
            Verified = false,
            Phone = request.Phone,
            Country = request.Country,
            PreferredCurrency = string.IsNullOrWhiteSpace(request.PreferredCurrency)
                ? "USD"
                : request.PreferredCurrency,
            Birthday = DateOnly.TryParse(request.Birthday, out var birthday)
                ? birthday
                : null
        };

        user.PasswordHash = passwordService.HashPassword(user, request.Password);

        store.Users.Add(user);

        var response = CreateAuthResponse(user);
        return Task.FromResult(response);
    }

    public async Task<AuthResponseDto> LoginWithGoogleAsync(
        GoogleLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var googleUser = await googleAuthService.VerifyIdTokenAsync(request.IdToken, cancellationToken);

        var user = store.Users.FirstOrDefault(x =>
            x.GoogleSubjectId == googleUser.SubjectId ||
            x.Email.Equals(googleUser.Email, StringComparison.OrdinalIgnoreCase));

        if (user is null)
        {
            user = new AppUser
            {
                Id = $"usr_{Guid.NewGuid():N}"[..12],
                FullName = googleUser.FullName,
                Email = googleUser.Email.Trim().ToLowerInvariant(),
                Verified = googleUser.EmailVerified,
                GoogleSubjectId = googleUser.SubjectId,
                PictureUrl = googleUser.PictureUrl,
                PreferredCurrency = "USD"
            };

            store.Users.Add(user);
        }
        else
        {
            if (user.IsBlocked)
            {
                throw new InvalidOperationException("User is blocked.");
            }

            user.GoogleSubjectId ??= googleUser.SubjectId;
            user.Verified = user.Verified || googleUser.EmailVerified;
            user.PictureUrl ??= googleUser.PictureUrl;
        }

        return CreateAuthResponse(user);
    }

    public Task<AuthResponseDto?> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = jwtTokenGenerator.HashRefreshToken(request.RefreshToken);

        var storedToken = store.RefreshTokens.FirstOrDefault(x =>
            x.TokenHash == tokenHash &&
            x.RevokedAtUtc is null &&
            x.ExpiresAtUtc > DateTime.UtcNow);

        if (storedToken is null)
        {
            return Task.FromResult<AuthResponseDto?>(null);
        }

        var user = store.Users.FirstOrDefault(x => x.Id == storedToken.UserId);
        if (user is null)
        {
            return Task.FromResult<AuthResponseDto?>(null);
        }

        storedToken.RevokedAtUtc = DateTime.UtcNow;

        var response = CreateAuthResponse(user);
        return Task.FromResult<AuthResponseDto?>(response);
    }

    private AuthResponseDto CreateAuthResponse(AppUser user)
    {
        var accessToken = jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        store.RefreshTokens.Add(new UserRefreshToken
        {
            Id = $"rt_{Guid.NewGuid():N}"[..12],
            UserId = user.Id,
            TokenHash = jwtTokenGenerator.HashRefreshToken(refreshToken),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays)
        });

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = user.ToDto()
        };
    }
}