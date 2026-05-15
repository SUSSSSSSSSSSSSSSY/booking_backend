using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Hotels;
using Booking.Contracts.Requests.Admin;
using Booking.Domain.Hotels;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef.Admin;

public class EfAdminHotelSubmissionService(BookingDbContext dbContext) : IAdminHotelSubmissionService
{
    public async Task<PagedResult<HotelSubmissionDto>> GetAllAsync(
        PaginationRequest pagination,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = dbContext.HotelSubmissions
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = status.Trim().ToLower();
            query = query.Where(x => x.Status.ToLower() == normalizedStatus);
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

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

    private static string GenerateRoomId()
    {
        return $"room_{Guid.NewGuid():N}"[..17];
    }

    public async Task<HotelSubmissionDto?> GetByIdAsync(
        string submissionId,
        CancellationToken cancellationToken = default)
    {
        var submission = await dbContext.HotelSubmissions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == submissionId, cancellationToken);

        return submission?.ToDto();
    }

    private async Task<HotelSubmissionDto> ApproveCreateAsync(
    string adminUserId,
    HotelSubmission submission,
    CancellationToken cancellationToken)
    {
        var hotelId = $"hot_{Guid.NewGuid():N}"[..16];

        var hotel = BuildHotelFromSubmission(submission, hotelId);

        hotel.OwnerUserId = submission.SubmittedByUserId;
        hotel.IsUserSubmitted = true;

        dbContext.Hotels.Add(hotel);

        submission.Status = "approved";
        submission.ApprovedHotelId = hotelId;
        submission.ReviewedAtUtc = DateTime.UtcNow;
        submission.ReviewedByAdminId = adminUserId;
        submission.AdminComment = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        return submission.ToDto();
    }

    private async Task<HotelSubmissionDto> ApproveUpdateAsync(
    string adminUserId,
    HotelSubmission submission,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(submission.TargetHotelId))
        {
            throw new InvalidOperationException("Target hotel id is required.");
        }

        var hotel = await dbContext.Hotels
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == submission.TargetHotelId, cancellationToken);

        if (hotel is null)
        {
            throw new InvalidOperationException("Target hotel not found.");
        }

        hotel.Name = submission.Name;
        hotel.City = submission.City;
        hotel.Country = submission.Country;
        hotel.Address = submission.Address;

        hotel.PricePerNight = submission.PricePerNight;
        hotel.DistanceToCenterKm = submission.DistanceToCenterKm;

        hotel.Tags = [.. submission.Tags];
        hotel.Amenities = [.. submission.Amenities];

        hotel.Description = submission.Description;
        hotel.Images = [.. submission.Images];

        hotel.ScoreItems = submission.ScoreItems.Select(x => new ScoreItem
        {
            Label = x.Label,
            Value = x.Value
        }).ToList();

        hotel.Facilities = submission.Facilities.Select(x => new FacilityGroup
        {
            Title = x.Title,
            Icon = x.Icon,
            Items = [.. x.Items]
        }).ToList();

        var existingRoomsById = hotel.Rooms
            .ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

        foreach (var oldRoom in hotel.Rooms)
        {
            oldRoom.IsDeleted = true;
        }

        foreach (var submittedRoom in submission.Rooms)
        {
            var requestedRoomId = submittedRoom.Id?.Trim();

            if (!string.IsNullOrWhiteSpace(requestedRoomId) &&
                existingRoomsById.TryGetValue(requestedRoomId, out var existingRoom))
            {
                existingRoom.Image = submittedRoom.Image;
                existingRoom.Name = submittedRoom.Name;
                existingRoom.Beds = submittedRoom.Beds;
                existingRoom.Price = submittedRoom.Price;
                existingRoom.FreeCancellation = submittedRoom.FreeCancellation;
                existingRoom.IsDeleted = false;

                continue;
            }

            hotel.Rooms.Add(new Room
            {
                Id = GenerateRoomId(),
                HotelId = hotel.Id,
                Image = submittedRoom.Image,
                Name = submittedRoom.Name,
                Beds = submittedRoom.Beds,
                Price = submittedRoom.Price,
                FreeCancellation = submittedRoom.FreeCancellation,
                IsDeleted = false
            });
        }

        submission.Status = "approved";
        submission.ApprovedHotelId = hotel.Id;
        submission.ReviewedAtUtc = DateTime.UtcNow;
        submission.ReviewedByAdminId = adminUserId;

        await dbContext.SaveChangesAsync(cancellationToken);

        return submission.ToDto();
    }

    private async Task<HotelSubmissionDto> ApproveDeleteAsync(
    string adminUserId,
    HotelSubmission submission,
    CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(submission.TargetHotelId))
        {
            throw new InvalidOperationException("Target hotel id is required.");
        }

        var hotel = await dbContext.Hotels
            .Include(x => x.Rooms)
            .FirstOrDefaultAsync(x => x.Id == submission.TargetHotelId, cancellationToken);

        if (hotel is null)
        {
            throw new InvalidOperationException("Target hotel not found.");
        }

        hotel.IsDeleted = true;

        foreach (var room in hotel.Rooms)
        {
            room.IsDeleted = true;
        }

        submission.Status = "approved";
        submission.ApprovedHotelId = hotel.Id;
        submission.ReviewedAtUtc = DateTime.UtcNow;
        submission.ReviewedByAdminId = adminUserId;

        await dbContext.SaveChangesAsync(cancellationToken);

        return submission.ToDto();
    }

    private static Hotel BuildHotelFromSubmission(HotelSubmission submission, string hotelId)
    {
        return new Hotel
        {
            Id = hotelId,

            Name = submission.Name,
            City = submission.City,
            Country = submission.Country,
            Address = submission.Address,

            PricePerNight = submission.PricePerNight,
            Rating = 0,
            ReviewCount = 0,
            DistanceToCenterKm = submission.DistanceToCenterKm,

            Tags = [.. submission.Tags],
            Amenities = [.. submission.Amenities],

            Description = submission.Description,
            Images = [.. submission.Images],

            ScoreItems = submission.ScoreItems.Select(x => new ScoreItem
            {
                Label = x.Label,
                Value = x.Value
            }).ToList(),

            Facilities = submission.Facilities.Select(x => new FacilityGroup
            {
                Title = x.Title,
                Icon = x.Icon,
                Items = [.. x.Items]
            }).ToList(),

            Rooms = submission.Rooms.Select(x => new Room
            {
                Id = $"room_{Guid.NewGuid():N}"[..17],
                HotelId = hotelId,
                Image = x.Image,
                Name = x.Name,
                Beds = x.Beds,
                Price = x.Price,
                FreeCancellation = x.FreeCancellation,
                IsDeleted = false
            }).ToList(),

            IsDeleted = false,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public async Task<HotelSubmissionDto?> ApproveAsync(
    string adminUserId,
    string submissionId,
    CancellationToken cancellationToken = default)
    {
        var submission = await dbContext.HotelSubmissions
            .FirstOrDefaultAsync(x => x.Id == submissionId, cancellationToken);

        if (submission is null)
        {
            return null;
        }

        if (submission.Status == "approved")
        {
            return submission.ToDto();
        }

        if (submission.Status == "rejected")
        {
            throw new InvalidOperationException("Rejected submission cannot be approved.");
        }

        return submission.SubmissionType switch
        {
            "create" => await ApproveCreateAsync(adminUserId, submission, cancellationToken),
            "update" => await ApproveUpdateAsync(adminUserId, submission, cancellationToken),
            "delete" => await ApproveDeleteAsync(adminUserId, submission, cancellationToken),
            _ => throw new InvalidOperationException("Unknown submission type.")
        };
    }

    public async Task<HotelSubmissionDto?> RejectAsync(
        string adminUserId,
        string submissionId,
        RejectHotelSubmissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var submission = await dbContext.HotelSubmissions
            .FirstOrDefaultAsync(x => x.Id == submissionId, cancellationToken);

        if (submission is null)
        {
            return null;
        }

        if (submission.Status == "approved")
        {
            throw new InvalidOperationException("Approved submission cannot be rejected.");
        }

        if (submission.Status == "rejected")
        {
            return submission.ToDto();
        }

        submission.Status = "rejected";
        submission.AdminComment = request.AdminComment;
        submission.ReviewedAtUtc = DateTime.UtcNow;
        submission.ReviewedByAdminId = adminUserId;

        await dbContext.SaveChangesAsync(cancellationToken);

        return submission.ToDto();
    }
}