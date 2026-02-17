namespace TenantKit.Core;

/// <summary>
/// A simple in-memory tenant store for development, testing, and small deployments.
/// Seed it via <see cref="TenantKitBuilder"/> or directly.
/// </summary>
public sealed class InMemoryTenantStore : ITenantStore
{
    private readonly Dictionary<string, ITenant> _tenants;

    public InMemoryTenantStore(IEnumerable<ITenant> tenants)
    {
        _tenants = tenants.ToDictionary(t => t.Id, StringComparer.OrdinalIgnoreCase);
    }

    public Task<ITenant?> FindByIdAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        _tenants.TryGetValue(tenantId, out var tenant);
        return Task.FromResult(tenant);
    }

    /// <summary>Returns all registered tenants (useful for admin endpoints).</summary>
    public IReadOnlyCollection<ITenant> All => _tenants.Values.ToList();
}
