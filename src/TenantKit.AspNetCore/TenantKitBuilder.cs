using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TenantKit.AspNetCore.Resolvers;
using TenantKit.Core;

namespace TenantKit.AspNetCore;

/// <summary>
/// Fluent builder for configuring TenantKit.
/// Obtained via <see cref="ServiceCollectionExtensions.AddTenantKit"/>.
/// </summary>
public sealed class TenantKitBuilder(IServiceCollection services)
{
    /// <summary>The underlying service collection.</summary>
    public IServiceCollection Services { get; } = services;

    // Factories are deferred so we can pick single vs composite after configure() runs
    internal readonly List<Func<IServiceProvider, ITenantResolver<HttpContext>>> ResolverFactories = [];

    // ──────────────────────────────────────────────
    // Resolver configuration
    // ──────────────────────────────────────────────

    /// <summary>Resolves tenant from the specified HTTP header (default: X-Tenant-Id).</summary>
    public TenantKitBuilder UseHeaderResolver(string headerName = "X-Tenant-Id")
    {
        ResolverFactories.Add(_ => new HeaderTenantResolver(headerName));
        return this;
    }

    /// <summary>Resolves tenant from the first subdomain segment (e.g. acme.myapp.com).</summary>
    public TenantKitBuilder UseSubdomainResolver(IEnumerable<string>? excludedSubdomains = null)
    {
        ResolverFactories.Add(_ => new SubdomainTenantResolver(excludedSubdomains));
        return this;
    }

    /// <summary>Resolves tenant from a query string parameter (default: tenant).</summary>
    public TenantKitBuilder UseQueryStringResolver(string paramName = "tenant")
    {
        ResolverFactories.Add(_ => new QueryStringTenantResolver(paramName));
        return this;
    }

    /// <summary>Resolves tenant from a JWT/cookie claim (default: tenant_id).</summary>
    public TenantKitBuilder UseClaimResolver(string claimType = "tenant_id")
    {
        ResolverFactories.Add(_ => new ClaimTenantResolver(claimType));
        return this;
    }

    /// <summary>Resolves tenant from a route value (default: tenantId).</summary>
    public TenantKitBuilder UseRouteValueResolver(string routeKey = "tenantId")
    {
        ResolverFactories.Add(_ => new RouteValueTenantResolver(routeKey));
        return this;
    }

    /// <summary>Provide a custom resolver.</summary>
    public TenantKitBuilder UseResolver<TResolver>()
        where TResolver : class, ITenantResolver<HttpContext>
    {
        ResolverFactories.Add(sp => sp.GetRequiredService<TResolver>());
        Services.AddSingleton<TResolver>();
        return this;
    }

    // ──────────────────────────────────────────────
    // Store configuration
    // ──────────────────────────────────────────────

    /// <summary>
    /// Seeds an in-memory tenant store. Perfect for development and small apps.
    /// </summary>
    public TenantKitBuilder WithInMemoryStore(Action<List<ITenant>> configure)
    {
        var tenants = new List<ITenant>();
        configure(tenants);
        Services.AddSingleton<ITenantStore>(new InMemoryTenantStore(tenants));
        return this;
    }

    /// <summary>Provide a custom store (DB, Redis, etc.).</summary>
    public TenantKitBuilder WithStore<TStore>()
        where TStore : class, ITenantStore
    {
        Services.AddScoped<ITenantStore, TStore>();
        return this;
    }

    // ──────────────────────────────────────────────
    // Behaviour
    // ──────────────────────────────────────────────

    /// <summary>Throw when no tenant can be resolved. Default: false.</summary>
    public TenantKitBuilder RequireTenant(bool require = true)
    {
        Services.PostConfigure<TenantKitOptions>(o => o.RequireTenant = require);
        return this;
    }

    /// <summary>Throw when the resolved tenant ID is unknown. Default: false.</summary>
    public TenantKitBuilder ThrowOnTenantNotFound(bool throwOnNotFound = true)
    {
        Services.PostConfigure<TenantKitOptions>(o => o.ThrowOnTenantNotFound = throwOnNotFound);
        return this;
    }
}
