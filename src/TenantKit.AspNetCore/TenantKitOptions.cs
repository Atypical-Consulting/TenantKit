namespace TenantKit.AspNetCore;

/// <summary>
/// Configuration options for TenantKit middleware behavior.
/// </summary>
public sealed class TenantKitOptions
{
    /// <summary>
    /// When true, throws <see cref="TenantKit.Core.TenantResolutionException"/> if no tenant can be resolved.
    /// Default: false (anonymous/public requests are allowed).
    /// </summary>
    public bool RequireTenant { get; set; } = false;

    /// <summary>
    /// When true, throws <see cref="TenantKit.Core.TenantNotFoundException"/> if the resolved tenant ID
    /// does not exist in the store. Default: false (unknown tenants silently pass as null).
    /// </summary>
    public bool ThrowOnTenantNotFound { get; set; } = false;
}
