using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Contracts.Dtos.Reviews;
using Booking.Contracts.Requests.Reviews;

namespace Booking.Application.Abstractions;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewDto>> GetByHotelIdAsync(string hotelId, CancellationToken cancellationToken = default);
    Task<ReviewDto> CreateAsync(string hotelId, CreateReviewRequest request, CancellationToken cancellationToken = default);
}