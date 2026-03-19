using TenantKit.Demo.Modules;
using TheAppManager.Startup;

// ──────────────────────────────────────────────────────────────────────────────
// TenantKit Demo — powered by TheAppManager module system
// Try it with:
//   curl http://localhost:5000/info -H "X-Tenant-Id: acme"
//   curl "http://localhost:5000/info?tenant=globex"
//   curl http://localhost:5000/info          (no tenant — public)
// ──────────────────────────────────────────────────────────────────────────────

AppManager.Start(args, modules =>
{
    modules
        .Add<TenantKitModule>()
        .Add<EndpointsModule>();
});
