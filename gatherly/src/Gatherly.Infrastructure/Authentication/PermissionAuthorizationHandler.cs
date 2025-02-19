using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Gatherly.Infrastructure.Authentication;

public class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        string? memberId = context.User.Claims.FirstOrDefault(
            c => c.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (!Guid.TryParse(memberId, out Guid parsedMemberId))
        {
            return;
        }

        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
        
        IPermissionService permissionService = serviceScope.ServiceProvider
            .GetRequiredService<IPermissionService>();

        HashSet<string> permissions =
            await permissionService.GetPermissionsAsync(parsedMemberId);

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
