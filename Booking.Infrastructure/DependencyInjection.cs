using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Infrastructure.Services;
using Booking.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var store = SeedDataFactory.Create();

        services.AddSingleton(store);

        services.AddSingleton<IHotelService, HotelService>();
        services.AddSingleton<IReviewService, ReviewService>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IBookingService, BookingService>();

        return services;
    }
}