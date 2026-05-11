using Booking.Application.Abstractions;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Hotels;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Services;

public class HotelService(InMemoryStore store) : IHotelService
{
    public Task<PagedResult<HotelDto>> GetAllAsync(
    PaginationRequest pagination,
    CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = store.Hotels
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.City)
            .ThenBy(x => x.Name)
            .AsQueryable();

        var totalCount = query.Count();

        var items = query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .Select(x => x.ToDto())
            .ToList();

        var result = PagedResult<HotelDto>.Create(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount);

        return Task.FromResult(result);
    }

    public Task<HotelDto?> GetByIdAsync(string hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = store.Hotels.FirstOrDefault(x => x.Id == hotelId);
        return Task.FromResult(hotel?.ToDto());
    }

    public Task<IReadOnlyList<HotelDto>> SearchAsync(string? city, string? country, CancellationToken cancellationToken = default)
    {
        var query = store.Hotels.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(x => x.City.Equals(city, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            query = query.Where(x => x.Country.Equals(country, StringComparison.OrdinalIgnoreCase));
        }

        var result = query
            .Select(x => x.ToDto())
            .ToList();

        return Task.FromResult<IReadOnlyList<HotelDto>>(result);
    }
}