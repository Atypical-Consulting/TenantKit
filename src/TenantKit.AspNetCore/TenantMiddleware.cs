using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TenantKit.Core;

namespace TenantKit.AspNetCore;

/// <summary>
/// ASP.NET Core middleware that resolves the current tenant for each request
/// and makes it available via <see cref="ITenantContext"/>.
/// </summary>
public sealed class TenantMiddleware(
    RequestDelegate next,
    ITenantResolver<HttpContext> resolver,
    ITenantStore store,
    IOptions<TenantKitOptions> options)
{
    private readonly TenantKitOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context, HttpTenantContext tenantContext)
    {
        var tenantId = await resolver.ResolveAsync(context, context.RequestAborted);

        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            var tenant = await store.FindByIdAsync(tenantId, context.RequestAborted);

            if (tenant is null && _options.ThrowOnTenantNotFound)
                throw new TenantNotFoundException(tenantId);

            tenantContext.Current = tenant;
        }
        else if (_options.RequireTenant)
        {
            throw new TenantResolutionException(
                $"Tenant resolution is required but no tenant could be resolved for '{context.Request.Path}'.");
        }

        await next(context);
    }
}
