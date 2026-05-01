using Booking.Contracts.Dtos.Admin;
using Booking.Domain.Hotels;
using Booking.Domain.Reviews;

namespace Booking.Infrastructure.Mappers;

public static class AdminReviewMapper
{
    public static AdminReviewDto ToAdminDto(this Review review, Hotel? hotel)
    {
        return new AdminReviewDto
        {
            Id = review.Id,
            HotelId = review.HotelId,
            HotelName = hotel?.Name,
            Author = review.Author,
            Rating = review.Rating,
            Text = review.Text,
            CreatedAtUtc = review.CreatedAtUtc,
            DaysAgo = Math.Max(0, (DateTime.UtcNow - review.CreatedAtUtc).Days)
        };
    }
}