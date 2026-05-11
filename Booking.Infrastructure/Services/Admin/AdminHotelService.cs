using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Hotels;
using Booking.Contracts.Requests.Admin;
using Booking.Domain.Hotels;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Storage;

namespace Booking.Infrastructure.Services.Admin;

public class AdminHotelService(InMemoryStore store) : IAdminHotelService
{
    public Task<PagedResult<HotelDto>> GetAllAsync(
    PaginationRequest pagination,
    CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = store.Hotels
            .OrderBy(x => x.IsDeleted)
            .ThenBy(x => x.City)
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

    public Task<HotelDto> CreateAsync(CreateHotelRequest request, CancellationToken cancellationToken = default)
    {
        var existing = store.Hotels.Any(x => x.Id == request.Id);

        if (existing)
        {
            throw new InvalidOperationException("Hotel with this id already exists.");
        }

        var hotel = new Hotel
        {
            Id = request.Id,
            Name = request.Name,
            City = request.City,
            Country = request.Country,
            Address = request.Address,
            PricePerNight = request.PricePerNight,
            Rating = request.Rating,
            ReviewCount = request.ReviewCount,
            DistanceToCenterKm = request.DistanceToCenterKm,
            Tags = [.. request.Tags],
            Amenities = [.. request.Amenities],
            Description = request.Description,
            Images = [.. request.Images],
            ScoreItems = request.ScoreItems.Select(x => new ScoreItem
            {
                Label = x.Label,
                Value = x.Value
            }).ToList(),
            Facilities = request.Facilities.Select(x => new FacilityGroup
            {
                Title = x.Title,
                Icon = x.Icon,
                Items = [.. x.Items]
            }).ToList(),
            Rooms = request.Rooms.Select(x => new Room
            {
                Id = x.Id,
                HotelId = request.Id,
                Image = x.Image,
                Name = x.Name,
                Beds = x.Beds,
                Price = x.Price,
                FreeCancellation = x.FreeCancellation
            }).ToList()
        };

        store.Hotels.Add(hotel);

        return Task.FromResult(hotel.ToDto());
    }

    public Task<HotelDto?> UpdateAsync(string hotelId, UpdateHotelRequest request, CancellationToken cancellationToken = default)
    {
        var hotel = store.Hotels.FirstOrDefault(x => x.Id == hotelId);

        if (hotel is null)
        {
            return Task.FromResult<HotelDto?>(null);
        }

        hotel.Name = request.Name;
        hotel.City = request.City;
        hotel.Country = request.Country;
        hotel.Address = request.Address;
        hotel.PricePerNight = request.PricePerNight;
        hotel.Rating = request.Rating;
        hotel.ReviewCount = request.ReviewCount;
        hotel.DistanceToCenterKm = request.DistanceToCenterKm;
        hotel.Tags = [.. request.Tags];
        hotel.Amenities = [.. request.Amenities];
        hotel.Description = request.Description;
        hotel.Images = [.. request.Images];

        hotel.ScoreItems = request.ScoreItems.Select(x => new ScoreItem
        {
            Label = x.Label,
            Value = x.Value
        }).ToList();

        hotel.Facilities = request.Facilities.Select(x => new FacilityGroup
        {
            Title = x.Title,
            Icon = x.Icon,
            Items = [.. x.Items]
        }).ToList();

        return Task.FromResult<HotelDto?>(hotel.ToDto());
    }

    public Task<bool> DeleteAsync(string hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = store.Hotels.FirstOrDefault(x => x.Id == hotelId);

        if (hotel is null)
        {
            return Task.FromResult(false);
        }

        store.Hotels.Remove(hotel);

        store.Reviews.RemoveAll(x => x.HotelId == hotelId);
        store.Bookings.RemoveAll(x => x.HotelId == hotelId);

        foreach (var user in store.Users)
        {
            user.Favorites.Remove(hotelId);
        }

        return Task.FromResult(true);
    }

    public Task<HotelDto?> AddRoomAsync(string hotelId, CreateRoomRequest request, CancellationToken cancellationToken = default)
    {
        var hotel = store.Hotels.FirstOrDefault(x => x.Id == hotelId);

        if (hotel is null)
        {
            return Task.FromResult<HotelDto?>(null);
        }

        var roomExists = hotel.Rooms.Any(x => x.Id == request.Id);

        if (roomExists)
        {
            throw new InvalidOperationException("Room with this id already exists.");
        }

        hotel.Rooms.Add(new Room
        {
            Id = request.Id,
            HotelId = hotelId,
            Image = request.Image,
            Name = request.Name,
            Beds = request.Beds,
            Price = request.Price,
            FreeCancellation = request.FreeCancellation
        });

        return Task.FromResult<HotelDto?>(hotel.ToDto());
    }

    public Task<HotelDto?> UpdateRoomAsync(
        string hotelId,
        string roomId,
        UpdateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        var hotel = store.Hotels.FirstOrDefault(x => x.Id == hotelId);

        if (hotel is null)
        {
            return Task.FromResult<HotelDto?>(null);
        }

        var room = hotel.Rooms.FirstOrDefault(x => x.Id == roomId);

        if (room is null)
        {
            return Task.FromResult<HotelDto?>(null);
        }

        room.Image = request.Image;
        room.Name = request.Name;
        room.Beds = request.Beds;
        room.Price = request.Price;
        room.FreeCancellation = request.FreeCancellation;

        return Task.FromResult<HotelDto?>(hotel.ToDto());
    }

    public Task<bool> DeleteRoomAsync(
        string hotelId,
        string roomId,
        CancellationToken cancellationToken = default)
    {
        var hotel = store.Hotels.FirstOrDefault(x => x.Id == hotelId);

        if (hotel is null)
        {
            return Task.FromResult(false);
        }

        var room = hotel.Rooms.FirstOrDefault(x => x.Id == roomId);

        if (room is null)
        {
            return Task.FromResult(false);
        }

        hotel.Rooms.Remove(room);

        return Task.FromResult(true);
    }
}