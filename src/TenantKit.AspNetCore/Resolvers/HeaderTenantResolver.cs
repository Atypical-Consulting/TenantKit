using Microsoft.AspNetCore.Http;
using TenantKit.Core;

namespace TenantKit.AspNetCore.Resolvers;

/// <summary>
/// Resolves the tenant from an HTTP request header.
/// Default header: <c>X-Tenant-Id</c>.
/// </summary>
public sealed class HeaderTenantResolver(string headerName = "X-Tenant-Id")
    : ITenantResolver<HttpContext>
{
    public Task<string?> ResolveAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var value = context.Request.Headers.TryGetValue(headerName, out var values)
            ? values.FirstOrDefault()
            : null;

        return Task.FromResult<string?>(value);
    }
}
