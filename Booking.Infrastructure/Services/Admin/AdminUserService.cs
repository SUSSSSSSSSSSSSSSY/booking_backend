using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Common;
using Booking.Contracts.Dtos.Admin;
using Booking.Contracts.Requests.Admin;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Storage;

namespace Booking.Infrastructure.Services.Admin;

public class AdminUserService(InMemoryStore store) : IAdminUserService
{
    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "User",
        "Admin"
    };

    public Task<PagedResult<AdminUserDto>> GetAllAsync(
        PaginationRequest pagination,
        CancellationToken cancellationToken = default)
    {
        pagination.Normalize();

        var query = store.Users
            .OrderByDescending(x => x.CreatedAtUtc)
            .AsQueryable();

        var totalCount = query.Count();

        var items = query
            .Skip(pagination.Skip)
            .Take(pagination.PageSize)
            .Select(x => x.ToAdminDto())
            .ToList();

        var result = PagedResult<AdminUserDto>.Create(
            items,
            pagination.Page,
            pagination.PageSize,
            totalCount);

        return Task.FromResult(result);
    }

    public Task<AdminUserDto?> GetByIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = store.Users.FirstOrDefault(x => x.Id == userId);

        return Task.FromResult(user?.ToAdminDto());
    }

    public Task<AdminUserDto?> BlockAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = store.Users.FirstOrDefault(x => x.Id == userId);

        if (user is null)
        {
            return Task.FromResult<AdminUserDto?>(null);
        }

        user.IsBlocked = true;

        return Task.FromResult<AdminUserDto?>(user.ToAdminDto());
    }

    public Task<AdminUserDto?> UnblockAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = store.Users.FirstOrDefault(x => x.Id == userId);

        if (user is null)
        {
            return Task.FromResult<AdminUserDto?>(null);
        }

        user.IsBlocked = false;

        return Task.FromResult<AdminUserDto?>(user.ToAdminDto());
    }

    public Task<AdminUserDto?> ChangeRoleAsync(
        string userId,
        ChangeUserRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = store.Users.FirstOrDefault(x => x.Id == userId);

        if (user is null)
        {
            return Task.FromResult<AdminUserDto?>(null);
        }

        if (string.IsNullOrWhiteSpace(request.Role) || !AllowedRoles.Contains(request.Role))
        {
            throw new InvalidOperationException("Invalid role. Allowed roles: User, Admin.");
        }

        user.Role = NormalizeRole(request.Role);

        return Task.FromResult<AdminUserDto?>(user.ToAdminDto());
    }

    private static string NormalizeRole(string role)
    {
        return role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            ? "Admin"
            : "User";
    }
}