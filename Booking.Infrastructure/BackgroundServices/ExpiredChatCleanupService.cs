using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Booking.Infrastructure.BackgroundServices;

public class ExpiredChatCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<ExpiredChatCleanupService> logger) : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(6);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Expired chat cleanup service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredChatsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while cleaning expired chats.");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }
    }

    private async Task CleanupExpiredChatsAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var deleteBeforeDate = today.AddDays(-14);

        var expiredThreads = await dbContext.ChatThreads
            .Join(
                dbContext.Bookings,
                thread => thread.BookingId,
                booking => booking.Id,
                (thread, booking) => new
                {
                    Thread = thread,
                    Booking = booking
                })
            .Where(x => x.Booking.CheckOut <= deleteBeforeDate)
            .Select(x => x.Thread)
            .ToListAsync(cancellationToken);

        if (expiredThreads.Count == 0)
        {
            logger.LogInformation("No expired chats found.");
            return;
        }

        dbContext.ChatThreads.RemoveRange(expiredThreads);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Deleted {Count} expired chat threads.",
            expiredThreads.Count);
    }
}