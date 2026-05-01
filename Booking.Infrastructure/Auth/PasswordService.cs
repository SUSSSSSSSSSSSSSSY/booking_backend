using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Application.Abstractions;
using Booking.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace Booking.Infrastructure.Auth;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<AppUser> _hasher = new();

    public string HashPassword(AppUser user, string password)
    {
        return _hasher.HashPassword(user, password);
    }

    public bool VerifyPassword(AppUser user, string password)
    {
        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return false;
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);

        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}