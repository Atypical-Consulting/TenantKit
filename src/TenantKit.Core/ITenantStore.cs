namespace TenantKit.Core;

/// <summary>
/// Looks up a tenant by its identifier.
/// Implement this to connect TenantKit to your own data store (DB, cache, config, etc.).
/// </summary>
public interface ITenantStore
{
    /// <summary>
    /// Retrieves a tenant by its identifier, or null if not found.
    /// </summary>
    Task<ITenant?> FindByIdAsync(string tenantId, CancellationToken cancellationToken = default);
}
