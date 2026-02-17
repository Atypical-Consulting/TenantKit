using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TenantKit.AspNetCore.Resolvers;
using TenantKit.Core;

namespace TenantKit.AspNetCore;

/// <summary>
/// Extension methods for registering TenantKit services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers TenantKit services and returns a builder for fluent configuration.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Services.AddTenantKit(tk => tk
    ///     .UseHeaderResolver()
    ///     .WithInMemoryStore(tenants =>
    ///     {
    ///         tenants.Add(Tenant.Create("acme", "Acme Corp"));
    ///         tenants.Add(Tenant.Create("globex", "Globex Inc"));
    ///     }));
    /// </code>
    /// </example>
    public static TenantKitBuilder AddTenantKit(
        this IServiceCollection services,
        Action<TenantKitBuilder>? configure = null)
    {
        // Options
        services.AddOptions<TenantKitOptions>();

        // HTTP context accessor
        services.AddHttpContextAccessor();

        // Scoped tenant context — one per request
        services.AddScoped<HttpTenantContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<HttpTenantContext>());

        var builder = new TenantKitBuilder(services);
        configure?.Invoke(builder);

        // Register resolver(s) — defer composite wrapping until now so we know the count
        if (builder.ResolverFactories.Count == 0)
        {
            // No resolver configured → fall back to header resolver (safest default)
            services.AddSingleton<ITenantResolver<HttpContext>, HeaderTenantResolver>();
        }
        else if (builder.ResolverFactories.Count == 1)
        {
            var factory = builder.ResolverFactories[0];
            services.AddSingleton<ITenantResolver<HttpContext>>(factory);
        }
        else
        {
            // Multiple resolvers → chain them via composite
            var factories = builder.ResolverFactories.ToList();
            services.AddSingleton<ITenantResolver<HttpContext>>(sp =>
                new CompositeTenantResolver(factories.Select(f => f(sp))));
        }

        return builder;
    }
}
