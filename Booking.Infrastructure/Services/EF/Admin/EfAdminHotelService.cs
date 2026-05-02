using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Dtos.Hotels;
using Booking.Contracts.Requests.Admin;
using Booking.Domain.Hotels;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef.Admin;

public class EfAdminHotelService(BookingDbContext dbContext) : IAdminHotelService
{
    public async Task<IReadOnlyList<HotelDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var hotels = await dbContext.Hotels
            .AsNoTracking()
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .OrderBy(x => x.IsDeleted)
            .ThenBy(x => x.City)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return hotels
            .Select(x => x.ToDto())
            .ToList();
    }

    public async Task<HotelDto?> GetByIdAsync(
        string hotelId,
        CancellationToken cancellationToken = default)
    {
        var hotel = await dbContext.Hotels
            .AsNoTracking()
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == hotelId, cancellationToken);

        return hotel?.ToDto();
    }

    public async Task<HotelDto> CreateAsync(
        CreateHotelRequest request,
        CancellationToken cancellationToken = default)
    {
        var hotelId = string.IsNullOrWhiteSpace(request.Id)
            ? $"hot_{Guid.NewGuid():N}"[..16]
            : request.Id.Trim();

        var exists = await dbContext.Hotels
            .AnyAsync(x => x.Id == hotelId, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Hotel with this id already exists.");
        }

        var hotel = new Hotel
        {
            Id = hotelId,
            Name = request.Name.Trim(),
            City = request.City.Trim(),
            Country = request.Country.Trim(),
            Address = request.Address,
            PricePerNight = request.PricePerNight,
            Rating = request.Rating,
            ReviewCount = request.ReviewCount,
            DistanceToCenterKm = request.DistanceToCenterKm,
            Tags = request.Tags?.ToList() ?? [],
            Amenities = request.Amenities?.ToList() ?? [],
            Description = request.Description,
            Images = request.Images?.ToList() ?? [],
            ScoreItems = request.ScoreItems?.Select(x => new ScoreItem
            {
                Label = x.Label,
                Value = x.Value
            }).ToList() ?? [],
            Facilities = request.Facilities?.Select(x => new FacilityGroup
            {
                Title = x.Title,
                Icon = x.Icon,
                Items = x.Items?.ToList() ?? []
            }).ToList() ?? [],
            Rooms = request.Rooms?.Select(x => new Room
            {
                Id = string.IsNullOrWhiteSpace(x.Id)
                    ? $"room_{Guid.NewGuid():N}"[..17]
                    : x.Id.Trim(),
                HotelId = hotelId,
                Image = x.Image,
                Name = x.Name,
                Beds = x.Beds,
                Price = x.Price,
                FreeCancellation = x.FreeCancellation,
                IsDeleted = false
            }).ToList() ?? [],
            IsDeleted = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Hotels.Add(hotel);

        await dbContext.SaveChangesAsync(cancellationToken);

        return hotel.ToDto();
    }

    public async Task<HotelDto?> UpdateAsync(
        string hotelId,
        UpdateHotelRequest request,
        CancellationToken cancellationToken = default)
    {
        var hotel = await dbContext.Hotels
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == hotelId, cancellationToken);

        if (hotel is null)
        {
            return null;
        }

        hotel.Name = request.Name.Trim();
        hotel.City = request.City.Trim();
        hotel.Country = request.Country.Trim();
        hotel.Address = request.Address;
        hotel.PricePerNight = request.PricePerNight;
        hotel.Rating = request.Rating;
        hotel.ReviewCount = request.ReviewCount;
        hotel.DistanceToCenterKm = request.DistanceToCenterKm;
        hotel.Tags = request.Tags?.ToList() ?? [];
        hotel.Amenities = request.Amenities?.ToList() ?? [];
        hotel.Description = request.Description;
        hotel.Images = request.Images?.ToList() ?? [];

        hotel.ScoreItems = request.ScoreItems?.Select(x => new ScoreItem
        {
            Label = x.Label,
            Value = x.Value
        }).ToList() ?? [];

        hotel.Facilities = request.Facilities?.Select(x => new FacilityGroup
        {
            Title = x.Title,
            Icon = x.Icon,
            Items = x.Items?.ToList() ?? []
        }).ToList() ?? [];

        await dbContext.SaveChangesAsync(cancellationToken);

        return hotel.ToDto();
    }

    public async Task<bool> DeleteAsync(
        string hotelId,
        CancellationToken cancellationToken = default)
    {
        var hotel = await dbContext.Hotels
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == hotelId, cancellationToken);

        if (hotel is null)
        {
            return false;
        }

        hotel.IsDeleted = true;

        foreach (var room in hotel.Rooms)
        {
            room.IsDeleted = true;
        }

        await RemoveHotelFromFavoritesAsync(hotelId, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<HotelDto?> AddRoomAsync(
        string hotelId,
        CreateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        var hotel = await dbContext.Hotels
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == hotelId && !x.IsDeleted, cancellationToken);

        if (hotel is null)
        {
            return null;
        }

        var roomId = string.IsNullOrWhiteSpace(request.Id)
            ? $"room_{Guid.NewGuid():N}"[..17]
            : request.Id.Trim();

        var roomExists = await dbContext.Rooms
            .AnyAsync(x => x.Id == roomId, cancellationToken);

        if (roomExists)
        {
            throw new InvalidOperationException("Room with this id already exists.");
        }

        hotel.Rooms.Add(new Room
        {
            Id = roomId,
            HotelId = hotelId,
            Image = request.Image,
            Name = request.Name,
            Beds = request.Beds,
            Price = request.Price,
            FreeCancellation = request.FreeCancellation,
            IsDeleted = false
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return hotel.ToDto();
    }

    public async Task<HotelDto?> UpdateRoomAsync(
        string hotelId,
        string roomId,
        UpdateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        var hotel = await dbContext.Hotels
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == hotelId && !x.IsDeleted, cancellationToken);

        if (hotel is null)
        {
            return null;
        }

        var room = hotel.Rooms.FirstOrDefault(x => x.Id == roomId && !x.IsDeleted);

        if (room is null)
        {
            return null;
        }

        room.Image = request.Image;
        room.Name = request.Name;
        room.Beds = request.Beds;
        room.Price = request.Price;
        room.FreeCancellation = request.FreeCancellation;

        await dbContext.SaveChangesAsync(cancellationToken);

        return hotel.ToDto();
    }

    public async Task<bool> DeleteRoomAsync(
        string hotelId,
        string roomId,
        CancellationToken cancellationToken = default)
    {
        var room = await dbContext.Rooms
            .FirstOrDefaultAsync(
                x => x.Id == roomId &&
                     x.HotelId == hotelId &&
                     !x.IsDeleted,
                cancellationToken);

        if (room is null)
        {
            return false;
        }

        room.IsDeleted = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task RemoveHotelFromFavoritesAsync(
        string hotelId,
        CancellationToken cancellationToken)
    {
        var usersWithFavorite = await dbContext.Users
            .Where(x => x.Favorites.Contains(hotelId))
            .ToListAsync(cancellationToken);

        foreach (var user in usersWithFavorite)
        {
            user.Favorites.Remove(hotelId);
        }
    }
}