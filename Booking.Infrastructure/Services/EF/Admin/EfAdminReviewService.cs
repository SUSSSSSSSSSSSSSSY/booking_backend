using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Admin;
using Booking.Domain.Reviews;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef.Admin;

public class EfAdminReviewService(BookingDbContext dbContext) : IAdminReviewService
{
    public async Task<PagedResult<AdminReviewDto>> GetAllAsync(
    PaginationRequest pagination,
    string? hotelId = null,
    CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = dbContext.Reviews
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(hotelId))
        {
            query = query.Where(x => x.HotelId == hotelId);
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(cancellationToken);

        var reviews = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var items = await MapReviewsAsync(reviews, cancellationToken);

        return PagedResult<AdminReviewDto>.Create(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount);
    }

    public async Task<AdminReviewDto?> GetByIdAsync(
        string reviewId,
        CancellationToken cancellationToken = default)
    {
        var review = await dbContext.Reviews
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == reviewId && !x.IsDeleted, cancellationToken);

        if (review is null)
        {
            return null;
        }

        return await MapReviewAsync(review, cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        string reviewId,
        CancellationToken cancellationToken = default)
    {
        var review = await dbContext.Reviews
            .FirstOrDefaultAsync(x => x.Id == reviewId && !x.IsDeleted, cancellationToken);

        if (review is null)
        {
            return false;
        }

        review.IsDeleted = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        await RecalculateHotelRatingAsync(review.HotelId, cancellationToken);

        return true;
    }

    private async Task<List<AdminReviewDto>> MapReviewsAsync(
        List<Review> reviews,
        CancellationToken cancellationToken)
    {
        var result = new List<AdminReviewDto>();

        foreach (var review in reviews)
        {
            var dto = await MapReviewAsync(review, cancellationToken);

            if (dto is not null)
            {
                result.Add(dto);
            }
        }

        return result;
    }

    private async Task<AdminReviewDto?> MapReviewAsync(
        Review review,
        CancellationToken cancellationToken)
    {
        var hotel = await dbContext.Hotels
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == review.HotelId, cancellationToken);

        return review.ToAdminDto(hotel);
    }

    private async Task RecalculateHotelRatingAsync(
        string hotelId,
        CancellationToken cancellationToken)
    {
        var hotel = await dbContext.Hotels
            .FirstOrDefaultAsync(x => x.Id == hotelId, cancellationToken);

        if (hotel is null)
        {
            return;
        }

        var reviews = await dbContext.Reviews
            .Where(x => x.HotelId == hotelId && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        hotel.ReviewCount = reviews.Count;

        hotel.Rating = reviews.Count == 0
            ? 0
            : Math.Round(reviews.Average(x => x.Rating), 1);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}