using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TenantKit.AspNetCore;
using TenantKit.Core;
using Xunit;

namespace TenantKit.Tests;

public sealed class MiddlewareIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly HttpClient _client;

    public MiddlewareIntegrationTests()
    {
        _host = new HostBuilder()
            .ConfigureWebHost(web =>
            {
                web.UseTestServer();
                web.ConfigureServices(services =>
                {
                    services.AddRouting();
                    services.AddTenantKit(tk => tk
                        .UseHeaderResolver()
                        .UseQueryStringResolver()
                        .WithInMemoryStore(tenants =>
                        {
                            tenants.Add(Tenant.Create("acme", "Acme Corp"));
                            tenants.Add(Tenant.Create("globex", "Globex Inc"));
                        }));
                });

                web.Configure(app =>
                {
                    app.UseRouting();
                    app.UseTenantKit();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet("/tenant", (ITenantContext ctx) =>
                            ctx.HasTenant ? ctx.Current!.Name : "no-tenant");

                        endpoints.MapGet("/tenant-id", (ITenantContext ctx) =>
                            ctx.HasTenant ? ctx.Current!.Id : "none");
                    });
                });
            })
            .Build();

        _host.Start();
        _client = _host.GetTestClient();
    }

    [Fact]
    public async Task RequestWithHeader_ResolvesTenant()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/tenant");
        request.Headers.Add("X-Tenant-Id", "acme");

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Be("Acme Corp");
    }

    [Fact]
    public async Task RequestWithQueryString_ResolvesTenant()
    {
        var response = await _client.GetAsync("/tenant?tenant=globex");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Be("Globex Inc");
    }

    [Fact]
    public async Task RequestWithNoTenant_ReturnsNoTenant()
    {
        var response = await _client.GetAsync("/tenant");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Be("no-tenant");
    }

    [Fact]
    public async Task RequestWithUnknownTenant_ReturnsNoTenant_WhenThrowNotConfigured()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/tenant");
        request.Headers.Add("X-Tenant-Id", "unknown-xyz");

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Be("no-tenant");
    }

    [Fact]
    public async Task Header_TakesPrecedence_OverQueryString()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/tenant-id?tenant=globex");
        request.Headers.Add("X-Tenant-Id", "acme");

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        body.Should().Be("acme");
    }

    public void Dispose()
    {
        _client.Dispose();
        _host.Dispose();
    }
}
