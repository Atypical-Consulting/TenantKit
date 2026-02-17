using Microsoft.AspNetCore.Http;
using TenantKit.Core;

namespace TenantKit.AspNetCore.Resolvers;

/// <summary>
/// Resolves the tenant from the first segment of the request host (subdomain).
/// Example: <c>acme.myapp.com</c> → tenant id = <c>acme</c>.
/// </summary>
/// <remarks>
/// Excludes common non-tenant subdomains like <c>www</c>, <c>api</c>, <c>app</c>.
/// </remarks>
public sealed class SubdomainTenantResolver(IEnumerable<string>? excludedSubdomains = null)
    : ITenantResolver<HttpContext>
{
    private static readonly HashSet<string> DefaultExclusions =
        new(["www", "api", "app", "mail", "ftp", "admin"], StringComparer.OrdinalIgnoreCase);

    private readonly HashSet<string> _excluded = excludedSubdomains is not null
        ? new HashSet<string>(excludedSubdomains, StringComparer.OrdinalIgnoreCase)
        : DefaultExclusions;

    public Task<string?> ResolveAsync(HttpContext context, CancellationToken cancellationToken = default)
    {
        var host = context.Request.Host.Host;
        var parts = host.Split('.');

        if (parts.Length < 3)
            return Task.FromResult<string?>(null);

        var subdomain = parts[0];
        var resolved = _excluded.Contains(subdomain) ? null : subdomain;

        return Task.FromResult<string?>(resolved);
    }
}
