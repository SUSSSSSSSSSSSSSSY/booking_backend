using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Infrastructure.Auth;
using Booking.Infrastructure.Persistence;
using Booking.Infrastructure.Services;
using Booking.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Booking.Application.Abstractions.Admin;
using Booking.Infrastructure.Services.Admin;

namespace Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var usePostgres = configuration.GetValue<bool>("Persistence:UsePostgres");

        if (usePostgres)
        {
            var connectionString = configuration.GetConnectionString("Default");

            services.AddDbContext<BookingDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            // Здесь позже заменим сервисы на EF-реализации:
            // services.AddScoped<IHotelService, EfHotelService>();
            // services.AddScoped<IUserService, EfUserService>();
            // ...
        }
        else
        {
            var store = SeedDataFactory.Create();
            services.AddSingleton(store);

            services.AddSingleton<IHotelService, HotelService>();
            services.AddSingleton<IReviewService, ReviewService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IBookingService, BookingService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IAdminHotelService, AdminHotelService>();
        }

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<IGoogleAuthService, GoogleAuthService>();
        

        return services;
    }
}