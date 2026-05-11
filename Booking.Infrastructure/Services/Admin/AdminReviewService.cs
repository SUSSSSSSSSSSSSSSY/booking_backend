using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Admin;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Storage;

namespace Booking.Infrastructure.Services.Admin;

public class AdminReviewService(InMemoryStore store) : IAdminReviewService
{
    public Task<PagedResult<AdminReviewDto>> GetAllAsync(
    PaginationRequest pagination,
    string? hotelId = null,
    CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = store.Reviews.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(hotelId))
        {
            query = query.Where(x => x.HotelId == hotelId);
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = query.Count();

        var items = query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .Select(MapReview)
            .ToList();

        var result = PagedResult<AdminReviewDto>.Create(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount);

        return Task.FromResult(result);
    }

    public Task<AdminReviewDto?> GetByIdAsync(
        string reviewId,
        CancellationToken cancellationToken = default)
    {
        var review = store.Reviews.FirstOrDefault(x => x.Id == reviewId);

        if (review is null)
        {
            return Task.FromResult<AdminReviewDto?>(null);
        }

        return Task.FromResult<AdminReviewDto?>(MapReview(review));
    }

    public Task<bool> DeleteAsync(
        string reviewId,
        CancellationToken cancellationToken = default)
    {
        var review = store.Reviews.FirstOrDefault(x => x.Id == reviewId);

        if (review is null)
        {
            return Task.FromResult(false);
        }

        store.Reviews.Remove(review);

        var hotel = store.Hotels.FirstOrDefault(x => x.Id == review.HotelId);

        if (hotel is not null)
        {
            var hotelReviews = store.Reviews
                .Where(x => x.HotelId == hotel.Id)
                .ToList();

            hotel.ReviewCount = hotelReviews.Count;

            hotel.Rating = hotelReviews.Count == 0
                ? 0
                : Math.Round(hotelReviews.Average(x => x.Rating), 1);
        }

        return Task.FromResult(true);
    }

    private AdminReviewDto MapReview(Booking.Domain.Reviews.Review review)
    {
        var hotel = store.Hotels.FirstOrDefault(x => x.Id == review.HotelId);
        return review.ToAdminDto(hotel);
    }
}