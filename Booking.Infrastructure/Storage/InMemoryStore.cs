using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Domain.Bookings;
using Booking.Domain.Hotels;
using Booking.Domain.Reviews;
using Booking.Domain.Users;

namespace Booking.Infrastructure.Storage;

public class InMemoryStore
{
    public List<Hotel> Hotels { get; } = [];
    public List<Review> Reviews { get; } = [];
    public List<AppUser> Users { get; } = [];
    public List<HotelBooking> Bookings { get; } = [];

    public List<UserRefreshToken> RefreshTokens { get; } = [];
}