using Microsoft.AspNetCore.Http;
using TenantKit.Core;

namespace TenantKit.AspNetCore.Resolvers;

/// <summary>
/// Resolves the tenant from a JWT/cookie claim.
/// Default claim type: <c>tenant_id</c>.
/// </summary>
public sealed class ClaimTenantResolver(string claimType = "tenant_id")
    : ITenantResolver<HttpContext>
{
    public Task<string?> ResolveAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var value = context.User?.FindFirst(claimType)?.Value;
        return Task.FromResult<string?>(value);
    }
}
