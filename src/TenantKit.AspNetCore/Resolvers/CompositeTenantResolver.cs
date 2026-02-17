using Microsoft.AspNetCore.Http;
using TenantKit.Core;

namespace TenantKit.AspNetCore.Resolvers;

/// <summary>
/// Tries multiple resolvers in order, returning the first non-null result.
/// Useful when your app supports multiple resolution strategies simultaneously.
/// </summary>
public sealed class CompositeTenantResolver(IEnumerable<ITenantResolver<HttpContext>> resolvers)
    : ITenantResolver<HttpContext>
{
    private readonly List<ITenantResolver<HttpContext>> _resolvers = resolvers.ToList();

    public async Task<string?> ResolveAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        foreach (var resolver in _resolvers)
        {
            var tenantId = await resolver.ResolveAsync(context, cancellationToken);
            if (!string.IsNullOrWhiteSpace(tenantId))
                return tenantId;
        }

        return null;
    }
}
