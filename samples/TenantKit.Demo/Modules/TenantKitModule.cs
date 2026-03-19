using TenantKit.AspNetCore;
using TenantKit.Core;
using TheAppManager.Modules;

namespace TenantKit.Demo.Modules;

/// <summary>
/// Configures TenantKit services and middleware.
/// </summary>
public sealed class TenantKitModule : IAppModule
{
    public void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddTenantKit(tk => tk
            // Strategy 1: check header first
            .UseHeaderResolver("X-Tenant-Id")
            // Strategy 2: fall back to query string (good for dev/testing)
            .UseQueryStringResolver("tenant")
            // Seed tenants (swap this for .WithStore<MyDbTenantStore>() in production)
            .WithInMemoryStore(tenants =>
            {
                tenants.Add(Tenant.Create("acme", "Acme Corp", new Dictionary<string, string>
                {
                    ["plan"]   = "enterprise",
                    ["region"] = "eu-west-1",
                    ["theme"]  = "dark"
                }));
                tenants.Add(Tenant.Create("globex", "Globex Inc", new Dictionary<string, string>
                {
                    ["plan"]   = "starter",
                    ["region"] = "us-east-1",
                    ["theme"]  = "light"
                }));
                tenants.Add(Tenant.Create("initech", "Initech Ltd", new Dictionary<string, string>
                {
                    ["plan"]   = "professional",
                    ["region"] = "ap-southeast-1",
                    ["theme"]  = "system"
                }));
            }));
    }

    public void ConfigureMiddleware(WebApplication app)
    {
        // TenantKit middleware — place it early
        app.UseTenantKit();
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
    {
    }
}
