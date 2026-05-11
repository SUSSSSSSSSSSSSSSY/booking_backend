using Booking.Application.Abstractions;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Hotels;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef;

public class EfHotelService(BookingDbContext dbContext) : IHotelService
{
    public async Task<PagedResult<HotelDto>> GetAllAsync(
    PaginationRequest pagination,
    CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = dbContext.Hotels
            .AsNoTracking()
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.City)
            .ThenBy(x => x.Name)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var hotels = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var items = hotels
            .Select(x => x.ToDto())
            .ToList();

        return PagedResult<HotelDto>.Create(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount);
    }

    public async Task<HotelDto?> GetByIdAsync(string hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = await dbContext.Hotels
            .AsNoTracking()
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == hotelId && !x.IsDeleted, cancellationToken);

        return hotel?.ToDto();
    }

    public async Task<IReadOnlyList<HotelDto>> SearchAsync(
        string? city,
        string? country,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Hotels
            .AsNoTracking()
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(city))
        {
            var normalizedCity = city.Trim().ToLower();
            query = query.Where(x => x.City.ToLower() == normalizedCity);
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            var normalizedCountry = country.Trim().ToLower();
            query = query.Where(x => x.Country.ToLower() == normalizedCountry);
        }

        var hotels = await query
            .OrderBy(x => x.City)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return hotels
            .Select(x => x.ToDto())
            .ToList();
    }
}