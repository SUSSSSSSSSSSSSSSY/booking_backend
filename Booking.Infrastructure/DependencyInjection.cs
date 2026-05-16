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
using Booking.Infrastructure.BackgroundServices;
using Npgsql;

namespace Booking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<IGoogleAuthService, GoogleAuthService>();

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
        services.AddScoped<IUserService, EfUserService>();
        services.AddScoped<IAuthService, EfAuthService>();
        services.AddScoped<IBookingService, EfBookingService>();
        services.AddScoped<IRoomAvailabilityService, EfRoomAvailabilityService>();
        services.AddScoped<IOwnerBookingService, EfOwnerBookingService>();
        services.AddScoped<IReviewService, EfReviewService>();

        services.AddScoped<IHotelSubmissionService, EfHotelSubmissionService>();

        services.AddScoped<IAdminHotelService, EfAdminHotelService>();
        services.AddScoped<IAdminUserService, EfAdminUserService>();
        services.AddScoped<IAdminBookingService, EfAdminBookingService>();
        services.AddScoped<IAdminReviewService, EfAdminReviewService>();
        services.AddScoped<IAdminHotelSubmissionService, EfAdminHotelSubmissionService>();

        services.AddScoped<IChatService, EfChatService>();

        services.AddHostedService<ExpiredChatCleanupService>();

        return services;
    }
}