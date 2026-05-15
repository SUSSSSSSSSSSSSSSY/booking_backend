using Booking.Application.Abstractions;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Bookings;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef;

public class EfOwnerBookingService(BookingDbContext dbContext) : IOwnerBookingService
{
    private static readonly string[] BlockingStatuses =
    [
        "pending_owner_approval",
        "confirmed"
    ];

    public async Task<PagedResult<BookingDto>> GetAllForOwnerAsync(
        string ownerUserId,
        PaginationRequest pagination,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = dbContext.Bookings
            .AsNoTracking()
            .Where(x => x.HotelOwnerUserId == ownerUserId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToLowerInvariant();

            query = query.Where(x => x.Status.ToLower() == normalizedStatus);
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var bookings = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var items = bookings
            .Select(x => x.ToDto())
            .ToList();

        return PagedResult<BookingDto>.Create(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount);
    }

    public async Task<PagedResult<BookingDto>> GetPendingForOwnerAsync(
        string ownerUserId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = dbContext.Bookings
            .AsNoTracking()
            .Where(x =>
                x.HotelOwnerUserId == ownerUserId &&
                x.Status == "pending_owner_approval")
            .OrderBy(x => x.CheckIn)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var bookings = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var items = bookings
            .Select(x => x.ToDto())
            .ToList();

        return PagedResult<BookingDto>.Create(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount);
    }

    public async Task<BookingDto?> GetByIdForOwnerAsync(
        string ownerUserId,
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await dbContext.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == bookingId &&
                     x.HotelOwnerUserId == ownerUserId,
                cancellationToken);

        return booking?.ToDto();
    }

    public async Task<BookingDto?> AcceptAsync(
        string ownerUserId,
        string bookingId,
        CancellationToken cancellationToken = default)
    {
        var booking = await dbContext.Bookings
            .FirstOrDefaultAsync(
                x => x.Id == bookingId &&
                     x.HotelOwnerUserId == ownerUserId,
                cancellationToken);

        if (booking is null)
        {
            return null;
        }

        if (booking.Status == "confirmed")
        {
            return booking.ToDto();
        }

        if (booking.Status != "pending_owner_approval")
        {
            throw new InvalidOperationException("Only pending booking can be accepted.");
        }

        var hasConfirmedOverlap = await dbContext.Bookings
            .AnyAsync(
                x => x.Id != booking.Id &&
                     x.RoomId == booking.RoomId &&
                     x.Status == "confirmed" &&
                     x.CheckIn < booking.CheckOut &&
                     booking.CheckIn < x.CheckOut,
                cancellationToken);

        if (hasConfirmedOverlap)
        {
            booking.Status = "rejected_by_owner";
            booking.OwnerRespondedAtUtc = DateTime.UtcNow;
            booking.CancellationReason = "Rejected automatically because the room is already booked for selected dates.";

            await dbContext.SaveChangesAsync(cancellationToken);

            throw new InvalidOperationException("Room is already booked for selected dates.");
        }

        booking.Status = "confirmed";
        booking.OwnerRespondedAtUtc = DateTime.UtcNow;
        booking.CancellationReason = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        return booking.ToDto();
    }

    public async Task<BookingDto?> RejectAsync(
        string ownerUserId,
        string bookingId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var booking = await dbContext.Bookings
            .FirstOrDefaultAsync(
                x => x.Id == bookingId &&
                     x.HotelOwnerUserId == ownerUserId,
                cancellationToken);

        if (booking is null)
        {
            return null;
        }

        if (booking.Status != "pending_owner_approval")
        {
            throw new InvalidOperationException("Only pending booking can be rejected.");
        }

        booking.Status = "rejected_by_owner";
        booking.OwnerRespondedAtUtc = DateTime.UtcNow;
        booking.CancelledAtUtc = DateTime.UtcNow;
        booking.CancellationReason = string.IsNullOrWhiteSpace(reason)
            ? "Rejected by hotel owner."
            : reason.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return booking.ToDto();
    }

    public async Task<BookingDto?> CancelAsync(
        string ownerUserId,
        string bookingId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var booking = await dbContext.Bookings
            .FirstOrDefaultAsync(
                x => x.Id == bookingId &&
                     x.HotelOwnerUserId == ownerUserId,
                cancellationToken);

        if (booking is null)
        {
            return null;
        }

        if (booking.Status is "cancelled_by_owner" or "cancelled_by_guest" or "rejected_by_owner")
        {
            return booking.ToDto();
        }

        booking.Status = "cancelled_by_owner";
        booking.CancelledAtUtc = DateTime.UtcNow;
        booking.CancellationReason = string.IsNullOrWhiteSpace(reason)
            ? "Cancelled by hotel owner."
            : reason.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return booking.ToDto();
    }
}