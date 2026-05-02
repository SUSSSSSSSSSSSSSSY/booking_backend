using Booking.Application.Abstractions;
using Booking.Application.Abstractions.Admin;
using Booking.Infrastructure.Auth;
using Booking.Infrastructure.Persistence;
using Booking.Infrastructure.Services;
using Booking.Infrastructure.Services.Admin;
using Booking.Infrastructure.Services.Ef;
using Booking.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Booking.Infrastructure.Services.Ef.Admin;
using Npgsql;

namespace Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var usePostgres = bool.TryParse(
            configuration["Persistence:UsePostgres"],
            out var parsedUsePostgres
        ) && parsedUsePostgres;

        var store = SeedDataFactory.Create();
        services.AddSingleton(store);

        if (usePostgres)
        {
            var connectionString = configuration.GetConnectionString("Default");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Default connection string is not configured.");
            }

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

            dataSourceBuilder.EnableDynamicJson();

            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<BookingDbContext>(options =>
            {
                options.UseNpgsql(dataSource);
            });

            services.AddScoped<IHotelService, EfHotelService>();
            services.AddScoped<IReviewService, EfReviewService>();
            services.AddScoped<IUserService, EfUserService>();
            services.AddScoped<IBookingService, EfBookingService>();
            services.AddScoped<IAuthService, EfAuthService>();

            services.AddScoped<IAdminHotelService, EfAdminHotelService>();
            services.AddScoped<IAdminUserService, EfAdminUserService>();
            services.AddScoped<IAdminBookingService, EfAdminBookingService>();
            services.AddScoped<IAdminReviewService, EfAdminReviewService>();
        }
        else
        {
            services.AddSingleton<IHotelService, HotelService>();
            services.AddSingleton<IReviewService, ReviewService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IBookingService, BookingService>();
            services.AddSingleton<IAuthService, AuthService>();

            services.AddSingleton<IAdminHotelService, AdminHotelService>();
            services.AddSingleton<IAdminUserService, AdminUserService>();
            services.AddSingleton<IAdminBookingService, AdminBookingService>();
            services.AddSingleton<IAdminReviewService, AdminReviewService>();
        }


        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<IGoogleAuthService, GoogleAuthService>();

        return services;
    }
}