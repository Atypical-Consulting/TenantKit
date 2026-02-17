namespace TenantKit.Core;

/// <summary>
/// Thrown when a tenant is required but could not be resolved.
/// </summary>
public sealed class TenantResolutionException(string message, Exception? inner = null)
    : Exception(message, inner);
