using Booking.Contracts.Dtos.Admin;
using Booking.Contracts.Requests.Admin;

namespace Booking.Application.Abstractions.Admin;

public interface IAdminUserService
{
    Task<IReadOnlyList<AdminUserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<AdminUserDto?> GetByIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<AdminUserDto?> BlockAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<AdminUserDto?> UnblockAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<AdminUserDto?> ChangeRoleAsync(
        string userId,
        ChangeUserRoleRequest request,
        CancellationToken cancellationToken = default);
}