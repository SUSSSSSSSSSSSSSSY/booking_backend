using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Application.Abstractions;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Booking.Infrastructure.Auth;

public class GoogleAuthService(IOptions<GoogleAuthOptions> options) : IGoogleAuthService
{
    private readonly GoogleAuthOptions _options = options.Value;

    public async Task<GoogleUserInfo> VerifyIdTokenAsync(
        string idToken,
        CancellationToken cancellationToken = default)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [_options.ClientId]
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

        if (!payload.EmailVerified)
        {
            throw new InvalidOperationException("Google email is not verified.");
        }

        return new GoogleUserInfo
        {
            SubjectId = payload.Subject,
            Email = payload.Email,
            EmailVerified = payload.EmailVerified,
            FullName = payload.Name ?? payload.Email,
            PictureUrl = payload.Picture
        };
    }
}