﻿using Microsoft.AspNetCore.Authorization;

namespace Gatherly.Infrastructure.Authentication;

public class PermissionRequirement(string permission)
    : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
