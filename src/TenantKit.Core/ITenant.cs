namespace TenantKit.Core;

/// <summary>
/// Represents a tenant in a multi-tenant application.
/// </summary>
public interface ITenant
{
    /// <summary>
    /// The unique identifier for the tenant.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The human-readable name of the tenant.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Optional dictionary of tenant-specific metadata (feature flags, config overrides, etc.).
    /// </summary>
    IReadOnlyDictionary<string, string> Metadata { get; }
}
