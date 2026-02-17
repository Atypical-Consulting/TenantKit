using TenantKit.AspNetCore;
using TenantKit.Core;

var builder = WebApplication.CreateBuilder(args);

// ──────────────────────────────────────────────────────────────────────────────
// TenantKit configuration
// Try it with:
//   curl http://localhost:5000/info -H "X-Tenant-Id: acme"
//   curl "http://localhost:5000/info?tenant=globex"
//   curl http://localhost:5000/info          (no tenant — public)
// ──────────────────────────────────────────────────────────────────────────────

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

var app = builder.Build();

// TenantKit middleware — place it early
app.UseTenantKit();

// ──────────────────────────────────────────────────────────────────────────────
// Endpoints
// ──────────────────────────────────────────────────────────────────────────────

// Public endpoint — no tenant required
app.MapGet("/", () => new
{
    service = "TenantKit Demo API",
    version = "0.1.0-poc",
    docs = "Pass X-Tenant-Id header or ?tenant= query param to identify your tenant"
});

// Tenant-aware endpoint
app.MapGet("/info", (ITenantContext ctx) =>
{
    if (!ctx.HasTenant)
        return Results.Ok(new { tenant = (string?)null, message = "Anonymous request — no tenant resolved" });

    var t = ctx.Current!;
    return Results.Ok(new
    {
        id       = t.Id,
        name     = t.Name,
        metadata = ((TenantKit.Core.Tenant)t).Metadata
    });
});

// Simulated per-tenant data endpoint
app.MapGet("/data", (ITenantContext ctx) =>
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
app.MapGet("/features", (ITenantContext ctx) =>
{
    if (!ctx.HasTenant)
        return Results.Unauthorized();

    var plan = ((TenantKit.Core.Tenant)ctx.Current!).Metadata?.GetValueOrDefault("plan", "free") ?? "free";

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
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
