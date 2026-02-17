using FluentAssertions;
using Microsoft.AspNetCore.Http;
using TenantKit.AspNetCore.Resolvers;
using Xunit;

namespace TenantKit.Tests;

public sealed class ResolverTests
{
    // ──────────────────────────────────────────────────────────────
    // Header resolver
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task HeaderResolver_HeaderPresent_ReturnsTenantId()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-Id"] = "acme";

        var resolver = new HeaderTenantResolver();
        var result = await resolver.ResolveAsync(context);

        result.Should().Be("acme");
    }

    [Fact]
    public async Task HeaderResolver_HeaderMissing_ReturnsNull()
    {
        var context = new DefaultHttpContext();

        var resolver = new HeaderTenantResolver();
        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    [Fact]
    public async Task HeaderResolver_CustomHeader_Works()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-My-Tenant"] = "globex";

        var resolver = new HeaderTenantResolver("X-My-Tenant");
        var result = await resolver.ResolveAsync(context);

        result.Should().Be("globex");
    }

    // ──────────────────────────────────────────────────────────────
    // Query string resolver
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task QueryStringResolver_ParamPresent_ReturnsTenantId()
    {
        var context = new DefaultHttpContext();
        context.Request.QueryString = new QueryString("?tenant=acme");

        var resolver = new QueryStringTenantResolver();
        var result = await resolver.ResolveAsync(context);

        result.Should().Be("acme");
    }

    [Fact]
    public async Task QueryStringResolver_ParamMissing_ReturnsNull()
    {
        var context = new DefaultHttpContext();

        var resolver = new QueryStringTenantResolver();
        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    // ──────────────────────────────────────────────────────────────
    // Subdomain resolver
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task SubdomainResolver_ValidSubdomain_ReturnsTenantId()
    {
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("acme.myapp.com");

        var resolver = new SubdomainTenantResolver();
        var result = await resolver.ResolveAsync(context);

        result.Should().Be("acme");
    }

    [Fact]
    public async Task SubdomainResolver_ExcludedSubdomain_ReturnsNull()
    {
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("www.myapp.com");

        var resolver = new SubdomainTenantResolver();
        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SubdomainResolver_NoSubdomain_ReturnsNull()
    {
        var context = new DefaultHttpContext();
        context.Request.Host = new HostString("myapp.com");

        var resolver = new SubdomainTenantResolver();
        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }

    // ──────────────────────────────────────────────────────────────
    // Composite resolver
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task CompositeResolver_FirstResolverWins()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-Id"] = "from-header";
        context.Request.QueryString = new QueryString("?tenant=from-query");

        var resolver = new CompositeTenantResolver(
        [
            new HeaderTenantResolver(),
            new QueryStringTenantResolver()
        ]);

        var result = await resolver.ResolveAsync(context);

        result.Should().Be("from-header");
    }

    [Fact]
    public async Task CompositeResolver_FallsThrough_WhenFirstFails()
    {
        var context = new DefaultHttpContext();
        // No header — only query
        context.Request.QueryString = new QueryString("?tenant=from-query");

        var resolver = new CompositeTenantResolver(
        [
            new HeaderTenantResolver(),
            new QueryStringTenantResolver()
        ]);

        var result = await resolver.ResolveAsync(context);

        result.Should().Be("from-query");
    }

    [Fact]
    public async Task CompositeResolver_AllFail_ReturnsNull()
    {
        var context = new DefaultHttpContext();

        var resolver = new CompositeTenantResolver(
        [
            new HeaderTenantResolver(),
            new QueryStringTenantResolver()
        ]);

        var result = await resolver.ResolveAsync(context);

        result.Should().BeNull();
    }
}
