using Booking.Contracts.Dtos.Hotels;
using Booking.Domain.Hotels;

namespace Booking.Infrastructure.Mappers;

public static class HotelSubmissionMapper
{
    public static HotelSubmissionDto ToDto(this HotelSubmission submission)
    {
        return new HotelSubmissionDto
        {
            Id = submission.Id,
            SubmittedByUserId = submission.SubmittedByUserId,

            SubmissionType = submission.SubmissionType,
            TargetHotelId = submission.TargetHotelId,
            ApprovedHotelId = submission.ApprovedHotelId,
            Status = submission.Status,

            Name = submission.Name,
            City = submission.City,
            Country = submission.Country,
            Address = submission.Address,

            PricePerNight = submission.PricePerNight,
            DistanceToCenterKm = submission.DistanceToCenterKm,

            Tags = [.. submission.Tags],
            Amenities = [.. submission.Amenities],

            Description = submission.Description,
            Images = [.. submission.Images],

            ScoreItems = submission.ScoreItems.Select(x => new ScoreItemDto
            {
                Label = x.Label,
                Value = x.Value
            }).ToList(),

            Facilities = submission.Facilities.Select(x => new FacilityGroupDto
            {
                Title = x.Title,
                Icon = x.Icon,
                Items = [.. x.Items]
            }).ToList(),

            Rooms = submission.Rooms.Select(x => new RoomDto
            {
                Id = x.Id,
                Image = x.Image,
                Name = x.Name,
                Beds = x.Beds,
                Price = x.Price,
                FreeCancellation = x.FreeCancellation
            }).ToList(),

            AdminComment = submission.AdminComment,
            CreatedAtUtc = submission.CreatedAtUtc,
            ReviewedAtUtc = submission.ReviewedAtUtc,
            ReviewedByAdminId = submission.ReviewedByAdminId
        };
    }
}