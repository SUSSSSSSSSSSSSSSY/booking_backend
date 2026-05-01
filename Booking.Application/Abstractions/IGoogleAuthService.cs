using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Abstractions;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo> VerifyIdTokenAsync(string idToken, CancellationToken cancellationToken = default);
}

public class GoogleUserInfo
{
    public string SubjectId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool EmailVerified { get; set; }
    public string FullName { get; set; } = default!;
    public string? PictureUrl { get; set; }
}