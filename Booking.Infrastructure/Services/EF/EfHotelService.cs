using Booking.Application.Abstractions;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Hotels;
using Booking.Contracts.Requests.Hotels;
using Booking.Domain.Hotels;
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

    public async Task<HotelDto?> GetByIdAsync(
        string hotelId,
        CancellationToken cancellationToken = default)
    {
        var hotel = await dbContext.Hotels
            .AsNoTracking()
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == hotelId && !x.IsDeleted, cancellationToken);

        return hotel?.ToDto();
    }

    public async Task<PagedResult<HotelDto>> SearchAsync(
        HotelSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        request.Normalize();

        var query = dbContext.Hotels
            .AsNoTracking()
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        query = ApplyDatabaseFilters(query, request);

        var hotels = await query.ToListAsync(cancellationToken);

        hotels = ApplyInMemoryFilters(hotels, request);

        hotels = ApplySorting(hotels, request.Sort);

        var totalCount = hotels.Count;

        var items = hotels
            .Skip(request.Skip)
            .Take(request.PageSize)
            .Select(x => x.ToDto())
            .ToList();

        return PagedResult<HotelDto>.Create(
            items,
            request.Page,
            request.PageSize,
            totalCount);
    }

    private static IQueryable<Hotel> ApplyDatabaseFilters(
        IQueryable<Hotel> query,
        HotelSearchRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            var city = request.City.Trim().ToLower();
            query = query.Where(x => x.City.ToLower() == city);
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            var country = request.Country.Trim().ToLower();
            query = query.Where(x => x.Country.ToLower() == country);
        }

        if (!string.IsNullOrWhiteSpace(request.Address))
        {
            var address = request.Address.Trim().ToLower();
            query = query.Where(x => x.Address != null && x.Address.ToLower().Contains(address));
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(x => x.PricePerNight >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(x => x.PricePerNight <= request.MaxPrice.Value);
        }

        if (request.MinRating.HasValue)
        {
            query = query.Where(x => x.Rating >= request.MinRating.Value);
        }

        if (request.MinReviewCount.HasValue)
        {
            query = query.Where(x => x.ReviewCount >= request.MinReviewCount.Value);
        }

        if (request.MaxDistanceToCenterKm.HasValue)
        {
            query = query.Where(x => x.DistanceToCenterKm <= request.MaxDistanceToCenterKm.Value);
        }

        if (request.MinRoomPrice.HasValue)
        {
            query = query.Where(x => x.Rooms.Any(r =>
                !r.IsDeleted &&
                r.Price >= request.MinRoomPrice.Value));
        }

        if (request.MaxRoomPrice.HasValue)
        {
            query = query.Where(x => x.Rooms.Any(r =>
                !r.IsDeleted &&
                r.Price <= request.MaxRoomPrice.Value));
        }

        if (request.FreeCancellation.HasValue)
        {
            query = query.Where(x => x.Rooms.Any(r =>
                !r.IsDeleted &&
                r.FreeCancellation == request.FreeCancellation.Value));
        }

        return query;
    }

    private static List<Hotel> ApplyInMemoryFilters(
        List<Hotel> hotels,
        HotelSearchRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var query = Normalize(request.Query);

            hotels = hotels
                .Where(x => MatchesGeneralQuery(x, query))
                .ToList();
        }

        var tags = ParseCsv(request.Tags);

        if (tags.Count > 0)
        {
            hotels = hotels
                .Where(x => MatchesStringList(x.Tags, tags, request.MatchAllTags))
                .ToList();
        }

        var amenities = ParseCsv(request.Amenities);

        if (amenities.Count > 0)
        {
            hotels = hotels
                .Where(x => MatchesStringList(x.Amenities, amenities, request.MatchAllAmenities))
                .ToList();
        }

        var facilities = ParseCsv(request.Facilities);

        if (facilities.Count > 0)
        {
            hotels = hotels
                .Where(x => MatchesFacilities(x, facilities, request.MatchAllFacilities))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.ScoreLabel))
        {
            var scoreLabel = Normalize(request.ScoreLabel);

            hotels = hotels
                .Where(x => x.ScoreItems.Any(s =>
                    Normalize(s.Label).Contains(scoreLabel) &&
                    (!request.MinScoreValue.HasValue || s.Value >= request.MinScoreValue.Value)))
                .ToList();
        }
        else if (request.MinScoreValue.HasValue)
        {
            hotels = hotels
                .Where(x => x.ScoreItems.Any(s => s.Value >= request.MinScoreValue.Value))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.RoomName))
        {
            var roomName = Normalize(request.RoomName);

            hotels = hotels
                .Where(x => x.Rooms.Any(r =>
                    !r.IsDeleted &&
                    Normalize(r.Name).Contains(roomName)))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.RoomBeds))
        {
            var roomBeds = Normalize(request.RoomBeds);

            hotels = hotels
                .Where(x => x.Rooms.Any(r =>
                    !r.IsDeleted &&
                    Normalize(r.Beds).Contains(roomBeds)))
                .ToList();
        }

        return hotels;
    }

    private static List<Hotel> ApplySorting(
        List<Hotel> hotels,
        string? sort)
    {
        var normalizedSort = Normalize(sort ?? "recommended");

        return normalizedSort switch
        {
            "priceasc" => hotels
                .OrderBy(x => x.PricePerNight)
                .ThenByDescending(x => x.Rating)
                .ToList(),

            "pricedesc" => hotels
                .OrderByDescending(x => x.PricePerNight)
                .ThenByDescending(x => x.Rating)
                .ToList(),

            "ratingasc" => hotels
                .OrderBy(x => x.Rating)
                .ThenBy(x => x.PricePerNight)
                .ToList(),

            "ratingdesc" => hotels
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.ReviewCount)
                .ToList(),

            "reviewsdesc" => hotels
                .OrderByDescending(x => x.ReviewCount)
                .ThenByDescending(x => x.Rating)
                .ToList(),

            "distanceasc" => hotels
                .OrderBy(x => x.DistanceToCenterKm)
                .ThenBy(x => x.PricePerNight)
                .ToList(),

            "distancecenter" => hotels
                .OrderBy(x => x.DistanceToCenterKm)
                .ThenBy(x => x.PricePerNight)
                .ToList(),

            "nameasc" => hotels
                .OrderBy(x => x.Name)
                .ToList(),

            "namedesc" => hotels
                .OrderByDescending(x => x.Name)
                .ToList(),

            "cityasc" => hotels
                .OrderBy(x => x.City)
                .ThenBy(x => x.Name)
                .ToList(),

            "recommended" => hotels
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.ReviewCount)
                .ThenBy(x => x.DistanceToCenterKm)
                .ThenBy(x => x.PricePerNight)
                .ToList(),

            _ => hotels
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.ReviewCount)
                .ThenBy(x => x.DistanceToCenterKm)
                .ThenBy(x => x.PricePerNight)
                .ToList()
        };
    }

    private static bool MatchesGeneralQuery(Hotel hotel, string query)
    {
        if (Normalize(hotel.Name).Contains(query))
        {
            return true;
        }

        if (Normalize(hotel.City).Contains(query))
        {
            return true;
        }

        if (Normalize(hotel.Country).Contains(query))
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(hotel.Address) &&
            Normalize(hotel.Address).Contains(query))
        {
            return true;
        }

        if (Normalize(hotel.Description).Contains(query))
        {
            return true;
        }

        if (hotel.Tags.Any(x => Normalize(x).Contains(query)))
        {
            return true;
        }

        if (hotel.Amenities.Any(x => Normalize(x).Contains(query)))
        {
            return true;
        }

        if (hotel.Facilities.Any(group =>
                Normalize(group.Title).Contains(query) ||
                Normalize(group.Icon).Contains(query) ||
                group.Items.Any(item => Normalize(item).Contains(query))))
        {
            return true;
        }

        if (hotel.ScoreItems.Any(score => Normalize(score.Label).Contains(query)))
        {
            return true;
        }

        if (hotel.Rooms.Any(room =>
                !room.IsDeleted &&
                (Normalize(room.Name).Contains(query) ||
                 Normalize(room.Beds).Contains(query))))
        {
            return true;
        }

        return false;
    }

    private static bool MatchesStringList(
        List<string> source,
        List<string> required,
        bool matchAll)
    {
        if (matchAll)
        {
            return required.All(requiredItem =>
                source.Any(sourceItem =>
                    Normalize(sourceItem).Contains(Normalize(requiredItem))));
        }

        return required.Any(requiredItem =>
            source.Any(sourceItem =>
                Normalize(sourceItem).Contains(Normalize(requiredItem))));
    }

    private static bool MatchesFacilities(
        Hotel hotel,
        List<string> required,
        bool matchAll)
    {
        var allFacilityTexts = hotel.Facilities
            .SelectMany(x =>
                new[] { x.Title, x.Icon }.Concat(x.Items))
            .Select(Normalize)
            .ToList();

        if (matchAll)
        {
            return required.All(requiredItem =>
                allFacilityTexts.Any(sourceItem =>
                    sourceItem.Contains(Normalize(requiredItem))));
        }

        return required.Any(requiredItem =>
            allFacilityTexts.Any(sourceItem =>
                sourceItem.Contains(Normalize(requiredItem))));
    }

    private static List<string> ParseCsv(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    private static string Normalize(string? value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }
}