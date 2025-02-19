using Gatherly.Domain.Entities;
using Gatherly.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gatherly.Infrastructure.Authentication;

public class PermissionService(ApplicationDbContext dbContext) : IPermissionService
{
    public async Task<HashSet<string>> GetPermissionsAsync(Guid memberId)
    {
        ICollection<Role>[] roles = await dbContext.Set<Member>()
            .Include(m => m.Roles)
            .ThenInclude(r => r.Permissions)
            .Where(m => m.Id == memberId)
            .Select(m => m.Roles)
            .ToArrayAsync();

        return roles
            .SelectMany(r => r)
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name)
            .ToHashSet();
    }
}
