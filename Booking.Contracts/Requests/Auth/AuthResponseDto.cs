using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Contracts.Dtos.Users;

namespace Booking.Contracts.Responses.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public UserProfileDto User { get; set; } = default!;
}