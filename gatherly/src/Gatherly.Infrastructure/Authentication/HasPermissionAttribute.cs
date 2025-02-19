using Gatherly.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Gatherly.Infrastructure.Authentication;

public sealed class HasPermissionAttribute(Permission permission)
    : AuthorizeAttribute(policy: permission.ToString());
