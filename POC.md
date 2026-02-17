# POC: TenantKit
**Date:** 2026-02-17
**Category:** API / NuGet package
**Time spent:** ~3h

## What it does

TenantKit is a zero-friction multi-tenancy middleware library for ASP.NET Core. It solves the most tedious problem every SaaS developer hits in .NET: resolving *which tenant* a request belongs to and making that information available throughout the DI container. It ships with 5 built-in resolver strategies (HTTP header, subdomain, query string, JWT/cookie claim, route value), a composite chain-of-responsibility resolver, an in-memory store for dev/testing, and a clean fluent builder API. One `AddTenantKit(...)` + one `UseTenantKit()` call and you're live.

## Target user

.NET SaaS developers building multi-tenant ASP.NET Core APIs or Blazor backends. The pain point: every SaaS app needs to know *who the tenant is* per-request, but the .NET ecosystem has no good, opinionated, zero-config solution for this. Developers either roll their own middleware (fragile, untested) or use Finbuckle.MultiTenant (heavy, EF-coupled, complex). TenantKit is the "add 3 lines and ship" answer.

## Revenue potential

- **NuGet (free core)** — TenantKit.Core + TenantKit.AspNetCore open source; drives awareness
- **TenantKit.EntityFrameworkCore** (paid NuGet) — Per-tenant DbContext, automatic query filters, connection string switching
- **TenantKit.Pro** (paid NuGet) — Redis-backed tenant cache, tenant isolation policies, rate limiting per tenant, audit logging
- **Consulting magnet** — Every Belgian/EU SaaS shop on .NET needs this; Atypical Consulting can lead with "we wrote the library"
- **Estimated realistic MRR if developed:** €300-1500 (NuGet sponsorships + Pro tier + 2-3 consulting contracts/year)

## Tech stack

- .NET 10, C# 13
- ASP.NET Core Minimal API (FrameworkReference — no heavy dependencies)
- xUnit 2.9.3 + FluentAssertions 7.2 + Microsoft.AspNetCore.TestHost
- Central Package Management (Directory.Packages.props)

## Demo / How to run

```bash
# Run the demo API
cd poc/2026-02-17-TenantKit
dotnet run --project samples/TenantKit.Demo --urls http://localhost:7888

# Test tenant resolution via header
curl http://localhost:7888/info -H "X-Tenant-Id: acme"

# Test via query string
curl "http://localhost:7888/info?tenant=globex"

# Anonymous request (no tenant)
curl http://localhost:7888/info

# Per-tenant data endpoint
curl http://localhost:7888/data -H "X-Tenant-Id: initech"

# Feature flags from tenant metadata
curl http://localhost:7888/features -H "X-Tenant-Id: acme"
```

Minimal integration (what library users write):
```csharp
builder.Services.AddTenantKit(tk => tk
    .UseHeaderResolver("X-Tenant-Id")
    .UseQueryStringResolver("tenant")   // fallback
    .WithInMemoryStore(tenants =>
    {
        tenants.Add(Tenant.Create("acme", "Acme Corp"));
        tenants.Add(Tenant.Create("globex", "Globex Inc"));
    }));

// ...
app.UseTenantKit();

// Anywhere in your endpoints:
app.MapGet("/data", (ITenantContext ctx) =>
{
    if (!ctx.HasTenant) return Results.Unauthorized();
    return Results.Ok(FetchData(ctx.Current!.Id));
});
```

## Test results

```
Total tests: 24
     Passed: 24
 Total time: 1.2 seconds
```

Tests cover: unit (Tenant record, InMemoryTenantStore, all 5 resolvers, composite resolver) + integration (middleware via TestHost: header resolution, query string resolution, fallback, unknown tenant, resolver priority).

## What's missing for v1

1. **TenantKit.EntityFrameworkCore** — Per-tenant DbContext factory, automatic `WHERE tenant_id = @tid` global query filters, per-tenant connection strings (biggest enterprise differentiator)
2. **GitHub repo + NuGet.org publish** — Push `TenantKit.Core` and `TenantKit.AspNetCore` to NuGet; add proper `<PackageMetadata>` (icon, description, SourceLink, MinVer)
3. **`RequireTenant` middleware short-circuit** — When `RequireTenant=true`, return HTTP 401/403 instead of throwing an exception (current behavior throws; fine for now but needs proper error response)

## Verdict

[x] 🔥 Strong — worth developing further

Clear pain point, no great .NET solution in the ecosystem, extremely fast to integrate (3 lines), natural upsell path from free NuGet → Pro tier → consulting. TaLibStandard proved Philippe can grow a .NET library to 89 stars — TenantKit is arguably more commercially focused.
