using Booking.Application.Abstractions;
using Booking.Contracts.Requests.Auth;
using Booking.Contracts.Responses.Auth;
using Booking.Domain.Users;
using Booking.Infrastructure.Auth;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Booking.Infrastructure.Services.Ef;

public class EfAuthService(
    BookingDbContext dbContext,
    IPasswordService passwordService,
    IJwtTokenGenerator jwtTokenGenerator,
    IGoogleAuthService googleAuthService,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponseDto?> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                x => x.Email.ToLower() == normalizedEmail,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        if (user.IsBlocked)
        {
            return null;
        }

        var passwordIsValid = passwordService.VerifyPassword(user, request.Password);

        if (!passwordIsValid)
        {
            return null;
        }

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await dbContext.Users
            .AnyAsync(
                x => x.Email.ToLower() == normalizedEmail,
                cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = new AppUser
        {
            Id = $"usr_{Guid.NewGuid():N}"[..12],
            FullName = request.FullName.Trim(),
            Email = normalizedEmail,
            Verified = false,
            Phone = request.Phone,
            Country = request.Country,
            PreferredCurrency = string.IsNullOrWhiteSpace(request.PreferredCurrency)
                ? "USD"
                : request.PreferredCurrency,
            Birthday = DateOnly.TryParse(request.Birthday, out var birthday)
                ? birthday
                : null,
            Role = "User",
            IsBlocked = false,
            Favorites = [],
            CreatedAtUtc = DateTime.UtcNow
        };

        user.PasswordHash = passwordService.HashPassword(user, request.Password);

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto> LoginWithGoogleAsync(
        GoogleLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var googleUser = await googleAuthService.VerifyIdTokenAsync(
            request.IdToken,
            cancellationToken);

        var normalizedEmail = googleUser.Email.Trim().ToLowerInvariant();

        var user = await dbContext.Users
            .FirstOrDefaultAsync(
                x => x.GoogleSubjectId == googleUser.SubjectId ||
                     x.Email.ToLower() == normalizedEmail,
                cancellationToken);

        if (user is null)
        {
            user = new AppUser
            {
                Id = $"usr_{Guid.NewGuid():N}"[..12],
                FullName = googleUser.FullName,
                Email = normalizedEmail,
                Verified = googleUser.EmailVerified,
                GoogleSubjectId = googleUser.SubjectId,
                PictureUrl = googleUser.PictureUrl,
                PreferredCurrency = "USD",
                Role = "User",
                IsBlocked = false,
                Favorites = [],
                CreatedAtUtc = DateTime.UtcNow
            };

            dbContext.Users.Add(user);
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

        await dbContext.SaveChangesAsync(cancellationToken);

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    public async Task<AuthResponseDto?> RefreshAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = jwtTokenGenerator.HashRefreshToken(request.RefreshToken);

        var storedToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash &&
                     x.RevokedAtUtc == null &&
                     x.ExpiresAtUtc > DateTime.UtcNow,
                cancellationToken);

        if (storedToken is null)
        {
            return null;
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == storedToken.UserId, cancellationToken);

        if (user is null || user.IsBlocked)
        {
            return null;
        }

        storedToken.RevokedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return await CreateAuthResponseAsync(user, cancellationToken);
    }

    private async Task<AuthResponseDto> CreateAuthResponseAsync(
        AppUser user,
        CancellationToken cancellationToken)
    {
        var accessToken = jwtTokenGenerator.GenerateAccessToken(user);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        var refreshTokenEntity = new UserRefreshToken
        {
            Id = $"rt_{Guid.NewGuid():N}"[..12],
            UserId = user.Id,
            TokenHash = jwtTokenGenerator.HashRefreshToken(refreshToken),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays),
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.RefreshTokens.Add(refreshTokenEntity);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = user.ToDto()
        };
    }
}