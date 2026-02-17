namespace TenantKit.Core;

/// <summary>
/// Provides access to the current tenant for the duration of a request.
/// Inject this into your services to access the resolved tenant.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// The current tenant. Null if no tenant was resolved (e.g. public endpoints).
    /// </summary>
    ITenant? Current { get; }

    /// <summary>
    /// Whether a tenant has been resolved for this request.
    /// </summary>
    bool HasTenant => Current is not null;
}
