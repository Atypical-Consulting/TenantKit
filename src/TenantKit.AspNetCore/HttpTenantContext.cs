using TenantKit.Core;

namespace TenantKit.AspNetCore;

/// <summary>
/// HTTP request-scoped implementation of <see cref="ITenantContext"/>.
/// Set by <see cref="TenantMiddleware"/> after resolving the tenant.
/// </summary>
public sealed class HttpTenantContext : ITenantContext
{
    /// <summary>The current tenant. May be null on public endpoints.</summary>
    public ITenant? Current { get; internal set; }

    /// <summary>Whether a tenant has been resolved for this request.</summary>
    public bool HasTenant => Current is not null;
}
