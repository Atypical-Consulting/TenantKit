using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace TenantKit.AspNetCore;

/// <summary>
/// Extension methods for registering TenantKit in the middleware pipeline.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds the TenantKit middleware to the pipeline.
    /// Must be called AFTER UseRouting() and BEFORE UseAuthentication() / UseAuthorization().
    /// </summary>
    public static IApplicationBuilder UseTenantKit(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantMiddleware>();
    }
}
