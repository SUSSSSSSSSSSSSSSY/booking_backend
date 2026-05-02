using Booking.Application.Abstractions;
using Booking.Contracts.Dtos.Reviews;
using Booking.Contracts.Requests.Reviews;
using Booking.Domain.Reviews;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef;

public class EfReviewService(BookingDbContext dbContext) : IReviewService
{
    public async Task<IReadOnlyList<ReviewDto>> GetByHotelIdAsync(
        string hotelId,
        CancellationToken cancellationToken = default)
    {
        var reviews = await dbContext.Reviews
            .AsNoTracking()
            .Where(x => x.HotelId == hotelId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return reviews
            .Select(x => x.ToDto())
            .ToList();
    }

    public async Task<ReviewDto> CreateAsync(
        string hotelId,
        CreateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var hotel = await dbContext.Hotels
            .FirstOrDefaultAsync(
                x => x.Id == hotelId && !x.IsDeleted,
                cancellationToken);

        if (hotel is null)
        {
            throw new InvalidOperationException("Hotel not found.");
        }

        if (request.Rating < 1 || request.Rating > 5)
        {
            throw new InvalidOperationException("Rating must be between 1 and 5.");
        }

        var review = new Review
        {
            Id = $"rev_{Guid.NewGuid():N}"[..12],
            HotelId = hotelId,
            UserId = null,
            Author = string.IsNullOrWhiteSpace(request.Author)
                ? "Anonymous"
                : request.Author.Trim(),
            Rating = request.Rating,
            Text = request.Text.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
            IsDeleted = false
        };

        dbContext.Reviews.Add(review);

        await dbContext.SaveChangesAsync(cancellationToken);

        await RecalculateHotelRatingAsync(hotelId, cancellationToken);

        return review.ToDto();
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