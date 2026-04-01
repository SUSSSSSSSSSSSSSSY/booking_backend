using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Application.Abstractions;
using Booking.Contracts.Dtos.Reviews;
using Booking.Contracts.Requests.Reviews;
using Booking.Domain.Reviews;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Storage;

namespace Booking.Infrastructure.Services;

public class ReviewService(InMemoryStore store) : IReviewService
{
    public Task<IReadOnlyList<ReviewDto>> GetByHotelIdAsync(string hotelId, CancellationToken cancellationToken = default)
    {
        var result = store.Reviews
            .Where(x => x.HotelId == hotelId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => x.ToDto())
            .ToList();

        return Task.FromResult<IReadOnlyList<ReviewDto>>(result);
    }

    public Task<ReviewDto> CreateAsync(string hotelId, CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        var review = new Review
        {
            Id = $"rev_{Guid.NewGuid():N}"[..12],
            HotelId = hotelId,
            Author = request.Author,
            Rating = request.Rating,
            Text = request.Text,
            CreatedAtUtc = DateTime.UtcNow
        };

        store.Reviews.Add(review);

        var hotel = store.Hotels.FirstOrDefault(x => x.Id == hotelId);
        if (hotel is not null)
        {
            var hotelReviews = store.Reviews.Where(x => x.HotelId == hotelId).ToList();
            hotel.ReviewCount = hotelReviews.Count;
            hotel.Rating = Math.Round(hotelReviews.Average(x => x.Rating), 1);
        }

        return Task.FromResult(review.ToDto());
    }
}