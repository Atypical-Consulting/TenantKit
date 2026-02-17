using Microsoft.AspNetCore.Http;
using TenantKit.Core;

namespace TenantKit.AspNetCore.Resolvers;

/// <summary>
/// Resolves the tenant from a query string parameter.
/// Default parameter: <c>tenant</c>.
/// </summary>
public sealed class QueryStringTenantResolver(string paramName = "tenant")
    : ITenantResolver<HttpContext>
{
    public Task<string?> ResolveAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var value = context.Request.Query.TryGetValue(paramName, out var values)
            ? values.FirstOrDefault()
            : null;

        return Task.FromResult<string?>(value);
    }
}
