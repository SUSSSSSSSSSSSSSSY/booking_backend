using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Contracts.Dtos.Hotels;

namespace Booking.Application.Abstractions;

public interface IHotelService
{
    Task<IReadOnlyList<HotelDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<HotelDto?> GetByIdAsync(string hotelId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HotelDto>> SearchAsync(string? city, string? country, CancellationToken cancellationToken = default);
}