using Booking.Application.Abstractions;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Hotels;
using Booking.Contracts.Requests.Hotels;
using Booking.Domain.Hotels;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef;

public class EfHotelSubmissionService(BookingDbContext dbContext) : IHotelSubmissionService
{
    public async Task<HotelSubmissionDto> CreateAsync(
        string userId,
        CreateHotelSubmissionRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        var userExists = await dbContext.Users
            .AnyAsync(x => x.Id == userId && !x.IsBlocked, cancellationToken);

        if (!userExists)
        {
            throw new InvalidOperationException("User not found or blocked.");
        }

        var submission = new HotelSubmission
        {
            Id = $"sub_{Guid.NewGuid():N}"[..16],
            SubmittedByUserId = userId,
            SubmissionType = "create",
            TargetHotelId = null,
            Status = "pending",

            Name = request.Name.Trim(),
            City = request.City.Trim(),
            Country = request.Country.Trim(),
            Address = request.Address?.Trim(),

            PricePerNight = request.PricePerNight,
            DistanceToCenterKm = request.DistanceToCenterKm,

            Tags = request.Tags?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList() ?? [],
            Amenities = request.Amenities?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList() ?? [],

            Description = request.Description.Trim(),
            Images = request.Images?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList() ?? [],

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
                HotelId = "",
                Image = x.Image,
                Name = x.Name,
                Beds = x.Beds,
                Price = x.Price,
                FreeCancellation = x.FreeCancellation
            }).ToList() ?? [],

            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.HotelSubmissions.Add(submission);

        await dbContext.SaveChangesAsync(cancellationToken);

        return submission.ToDto();
    }

    public async Task<PagedResult<HotelSubmissionDto>> GetMySubmissionsAsync(
        string userId,
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = dbContext.HotelSubmissions
            .AsNoTracking()
            .Where(x => x.SubmittedByUserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var submissions = await query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        var items = submissions
            .Select(x => x.ToDto())
            .ToList();

        return PagedResult<HotelSubmissionDto>.Create(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount);
    }

    public async Task<HotelSubmissionDto?> GetMySubmissionByIdAsync(
        string userId,
        string submissionId,
        CancellationToken cancellationToken = default)
    {
        var submission = await dbContext.HotelSubmissions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == submissionId &&
                     x.SubmittedByUserId == userId,
                cancellationToken);

        return submission?.ToDto();
    }

    private static void ValidateRequest(CreateHotelSubmissionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Hotel name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.City))
        {
            throw new InvalidOperationException("City is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Country))
        {
            throw new InvalidOperationException("Country is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new InvalidOperationException("Description is required.");
        }

        if (request.PricePerNight <= 0)
        {
            throw new InvalidOperationException("Price per night must be greater than zero.");
        }

        if (request.Rooms is null || request.Rooms.Count == 0)
        {
            throw new InvalidOperationException("At least one room is required.");
        }

        foreach (var room in request.Rooms)
        {
            if (string.IsNullOrWhiteSpace(room.Name))
            {
                throw new InvalidOperationException("Room name is required.");
            }

            if (room.Price <= 0)
            {
                throw new InvalidOperationException("Room price must be greater than zero.");
            }
        }
    }
    public async Task<PagedResult<HotelDto>> GetMyHotelsAsync(
    string userId,
    PaginationRequest pagination,
    CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = dbContext.Hotels
            .AsNoTracking()
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .Where(x => x.OwnerUserId == userId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
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

    public async Task<HotelSubmissionDto> SubmitUpdateAsync(
    string userId,
    string hotelId,
    UpdateOwnedHotelSubmissionRequest request,
    CancellationToken cancellationToken = default)
    {
        var hotel = await dbContext.Hotels
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == hotelId &&
                     x.OwnerUserId == userId &&
                     !x.IsDeleted,
                cancellationToken);

        if (hotel is null)
        {
            throw new InvalidOperationException("Hotel not found or you are not the owner.");
        }

        var hasPendingSubmission = await dbContext.HotelSubmissions
            .AnyAsync(
                x => x.TargetHotelId == hotelId &&
                     x.Status == "pending",
                cancellationToken);

        if (hasPendingSubmission)
        {
            throw new InvalidOperationException("This hotel already has a pending moderation request.");
        }

        ValidateUpdateRequest(request);

        var submission = new HotelSubmission
        {
            Id = $"sub_{Guid.NewGuid():N}"[..16],
            SubmittedByUserId = userId,
            SubmissionType = "update",
            TargetHotelId = hotelId,
            Status = "pending",

            Name = request.Name.Trim(),
            City = request.City.Trim(),
            Country = request.Country.Trim(),
            Address = request.Address?.Trim(),

            PricePerNight = request.PricePerNight,
            DistanceToCenterKm = request.DistanceToCenterKm,

            Tags = request.Tags?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList() ?? [],
            Amenities = request.Amenities?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList() ?? [],

            Description = request.Description.Trim(),
            Images = request.Images?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList() ?? [],

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
                FreeCancellation = x.FreeCancellation
            }).ToList() ?? [],

            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.HotelSubmissions.Add(submission);

        await dbContext.SaveChangesAsync(cancellationToken);

        return submission.ToDto();
    }

    public async Task<HotelSubmissionDto> SubmitDeleteAsync(
    string userId,
    string hotelId,
    DeleteOwnedHotelSubmissionRequest request,
    CancellationToken cancellationToken = default)
    {
        var hotel = await dbContext.Hotels
            .AsNoTracking()
            .Include(x => x.Rooms.Where(r => !r.IsDeleted))
            .FirstOrDefaultAsync(
                x => x.Id == hotelId &&
                     x.OwnerUserId == userId &&
                     !x.IsDeleted,
                cancellationToken);

        if (hotel is null)
        {
            throw new InvalidOperationException("Hotel not found or you are not the owner.");
        }

        var hasPendingSubmission = await dbContext.HotelSubmissions
            .AnyAsync(
                x => x.TargetHotelId == hotelId &&
                     x.Status == "pending",
                cancellationToken);

        if (hasPendingSubmission)
        {
            throw new InvalidOperationException("This hotel already has a pending moderation request.");
        }

        var submission = new HotelSubmission
        {
            Id = $"sub_{Guid.NewGuid():N}"[..16],
            SubmittedByUserId = userId,
            SubmissionType = "delete",
            TargetHotelId = hotelId,
            Status = "pending",

            Name = hotel.Name,
            City = hotel.City,
            Country = hotel.Country,
            Address = hotel.Address,

            PricePerNight = hotel.PricePerNight,
            DistanceToCenterKm = hotel.DistanceToCenterKm,

            Tags = [.. hotel.Tags],
            Amenities = [.. hotel.Amenities],

            Description = hotel.Description,
            Images = [.. hotel.Images],

            ScoreItems = hotel.ScoreItems.Select(x => new ScoreItem
            {
                Label = x.Label,
                Value = x.Value
            }).ToList(),

            Facilities = hotel.Facilities.Select(x => new FacilityGroup
            {
                Title = x.Title,
                Icon = x.Icon,
                Items = [.. x.Items]
            }).ToList(),

            Rooms = hotel.Rooms.Select(x => new Room
            {
                Id = x.Id,
                HotelId = hotelId,
                Image = x.Image,
                Name = x.Name,
                Beds = x.Beds,
                Price = x.Price,
                FreeCancellation = x.FreeCancellation
            }).ToList(),

            AdminComment = request.Reason,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.HotelSubmissions.Add(submission);

        await dbContext.SaveChangesAsync(cancellationToken);

        return submission.ToDto();
    }

    private static void ValidateUpdateRequest(UpdateOwnedHotelSubmissionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Hotel name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.City))
        {
            throw new InvalidOperationException("City is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Country))
        {
            throw new InvalidOperationException("Country is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            throw new InvalidOperationException("Description is required.");
        }

        if (request.PricePerNight <= 0)
        {
            throw new InvalidOperationException("Price per night must be greater than zero.");
        }

        if (request.Rooms is null || request.Rooms.Count == 0)
        {
            throw new InvalidOperationException("At least one room is required.");
        }

        foreach (var room in request.Rooms)
        {
            if (string.IsNullOrWhiteSpace(room.Name))
            {
                throw new InvalidOperationException("Room name is required.");
            }

            if (room.Price <= 0)
            {
                throw new InvalidOperationException("Room price must be greater than zero.");
            }
        }
    }


}