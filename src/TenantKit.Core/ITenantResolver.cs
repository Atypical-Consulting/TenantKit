namespace TenantKit.Core;

/// <summary>
/// Resolves the current tenant from an HTTP context or request context string.
/// </summary>
public interface ITenantResolver<TContext>
{
    /// <summary>
    /// Attempts to resolve the tenant identifier from the provided context.
    /// Returns null if the tenant cannot be determined.
    /// </summary>
    Task<string?> ResolveAsync(TContext context, CancellationToken cancellationToken = default);
}
