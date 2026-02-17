namespace TenantKit.Core;

/// <summary>
/// Default immutable implementation of <see cref="ITenant"/>.
/// </summary>
public sealed record Tenant(
    string Id,
    string Name,
    IReadOnlyDictionary<string, string>? Metadata = null) : ITenant
{
    IReadOnlyDictionary<string, string> ITenant.Metadata =>
        Metadata ?? new Dictionary<string, string>();

    /// <summary>Creates a simple tenant with no metadata.</summary>
    public static Tenant Create(string id, string name) => new(id, name);

    /// <summary>Creates a tenant with metadata.</summary>
    public static Tenant Create(string id, string name, Dictionary<string, string> metadata) =>
        new(id, name, metadata);
}
