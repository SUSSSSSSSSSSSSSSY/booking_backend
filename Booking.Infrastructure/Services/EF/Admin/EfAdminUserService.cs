using Booking.Application.Abstractions.Admin;
using Booking.Contracts.Dtos.Admin;
using Booking.Contracts.Requests.Admin;
using Booking.Infrastructure.Mappers;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Services.Ef.Admin;

public class EfAdminUserService(BookingDbContext dbContext) : IAdminUserService
{
    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "User",
        "Admin"
    };

    public async Task<IReadOnlyList<AdminUserDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return users
            .Select(x => x.ToAdminDto())
            .ToList();
    }

    public async Task<AdminUserDto?> GetByIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        return user?.ToAdminDto();
    }

    public async Task<AdminUserDto?> BlockAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        user.IsBlocked = true;

        await dbContext.SaveChangesAsync(cancellationToken);

        return user.ToAdminDto();
    }

    public async Task<AdminUserDto?> UnblockAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        user.IsBlocked = false;

        await dbContext.SaveChangesAsync(cancellationToken);

        return user.ToAdminDto();
    }

    public async Task<AdminUserDto?> ChangeRoleAsync(
        string userId,
        ChangeUserRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(request.Role) || !AllowedRoles.Contains(request.Role))
        {
            throw new InvalidOperationException("Invalid role. Allowed roles: User, Admin.");
        }

        user.Role = NormalizeRole(request.Role);

        await dbContext.SaveChangesAsync(cancellationToken);

        return user.ToAdminDto();
    }

    private static string NormalizeRole(string role)
    {
        return role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            ? "Admin"
            : "User";
    }
}