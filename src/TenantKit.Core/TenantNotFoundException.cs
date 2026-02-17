namespace TenantKit.Core;

/// <summary>
/// Thrown when a resolved tenant ID does not match any known tenant.
/// </summary>
public sealed class TenantNotFoundException(string tenantId)
    : Exception($"Tenant '{tenantId}' was not found.")
{
    public string TenantId { get; } = tenantId;
}
