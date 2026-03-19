using TenantKit.Core;
using TheAppManager.Modules;

namespace TenantKit.Demo.Modules;

/// <summary>
/// Maps all API endpoints for the TenantKit Demo.
/// </summary>
public sealed class EndpointsModule : IAppModule
{
    public void ConfigureServices(WebApplicationBuilder builder)
    {
    }

    public void ConfigureMiddleware(WebApplication app)
    {
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
        // Public endpoint — no tenant required
        endpoints.MapGet("/", () => new
        {
            service = "TenantKit Demo API",
            version = "0.1.0-poc",
            docs = "Pass X-Tenant-Id header or ?tenant= query param to identify your tenant"
        });

        // Tenant-aware endpoint
        endpoints.MapGet("/info", (ITenantContext ctx) =>
        {
            if (!ctx.HasTenant)
                return Results.Ok(new { tenant = (string?)null, message = "Anonymous request — no tenant resolved" });

            var t = ctx.Current!;
            return Results.Ok(new
            {
                id       = t.Id,
                name     = t.Name,
                metadata = ((Tenant)t).Metadata
            });
        });

        // Simulated per-tenant data endpoint
        endpoints.MapGet("/data", (ITenantContext ctx) =>
        {
            if (!ctx.HasTenant)
                return Results.Unauthorized();

            // In a real app: query YOUR database filtered by tenant.Id
            var rows = ctx.Current!.Id switch
            {
                "acme"    => new[] { "Widget A", "Widget B", "Widget C" },
                "globex"  => new[] { "Product X", "Product Y" },
                "initech" => new[] { "Item Alpha" },
                _         => Array.Empty<string>()
            };

            return Results.Ok(new { tenant = ctx.Current.Id, records = rows, count = rows.Length });
        });

        // Feature flag endpoint (reads from tenant metadata)
        endpoints.MapGet("/features", (ITenantContext ctx) =>
        {
            if (!ctx.HasTenant)
                return Results.Unauthorized();

            var plan = ((Tenant)ctx.Current!).Metadata?.GetValueOrDefault("plan", "free") ?? "free";

            return Results.Ok(new
            {
                tenant   = ctx.Current.Id,
                plan,
                features = plan switch
                {
                    "enterprise"   => new[] { "SSO", "Audit Logs", "Custom Domain", "SLA 99.99%", "Dedicated Support" },
                    "professional" => new[] { "SSO", "Audit Logs", "Custom Domain" },
                    "starter"      => new[] { "Standard Support" },
                    _              => new[] { "Limited Access" }
                }
            });
        });

        // Health check (tenant-agnostic)
        endpoints.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
    }
}
