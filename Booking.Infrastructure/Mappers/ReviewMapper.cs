using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booking.Contracts.Dtos.Reviews;
using Booking.Domain.Reviews;

namespace Booking.Infrastructure.Mappers;

public static class ReviewMapper
{
    public static ReviewDto ToDto(this Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            Author = review.Author,
            HotelId = review.HotelId,
            Rating = review.Rating,
            DaysAgo = Math.Max(0, (DateTime.UtcNow - review.CreatedAtUtc).Days),
            Text = review.Text
        };
    }
}