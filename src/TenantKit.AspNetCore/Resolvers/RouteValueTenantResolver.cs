using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TenantKit.Core;

namespace TenantKit.AspNetCore.Resolvers;

/// <summary>
/// Resolves the tenant from a route value.
/// Example: <c>/api/{tenantId}/resources</c>.
/// </summary>
public sealed class RouteValueTenantResolver(string routeKey = "tenantId")
    : ITenantResolver<HttpContext>
{
    public Task<string?> ResolveAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var value = context.GetRouteValue(routeKey)?.ToString();
        return Task.FromResult<string?>(value);
    }
}
