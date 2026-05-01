using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Domain.Users;

namespace Booking.Application.Abstractions;

public interface IPasswordService
{
    string HashPassword(AppUser user, string password);
    bool VerifyPassword(AppUser user, string password);
}