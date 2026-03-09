using Shouldly;
using TenantKit.Core;
using Xunit;

namespace TenantKit.Tests;

public sealed class InMemoryTenantStoreTests
{
    private readonly InMemoryTenantStore _store;

    public InMemoryTenantStoreTests()
    {
        _store = new InMemoryTenantStore(
        [
            Tenant.Create("acme", "Acme Corp"),
            Tenant.Create("globex", "Globex Inc"),
        ]);
    }

    [Fact]
    public async Task FindByIdAsync_KnownTenant_ReturnsTenant()
    {
        var tenant = await _store.FindByIdAsync("acme");

        tenant.ShouldNotBeNull();
        tenant!.Name.ShouldBe("Acme Corp");
    }

    [Fact]
    public async Task FindByIdAsync_UnknownTenant_ReturnsNull()
    {
        var tenant = await _store.FindByIdAsync("unknown");

        tenant.ShouldBeNull();
    }

    [Fact]
    public async Task FindByIdAsync_IsCaseInsensitive()
    {
        var tenant = await _store.FindByIdAsync("ACME");

        tenant.ShouldNotBeNull();
        tenant!.Id.ShouldBe("acme");
    }

    [Fact]
    public void All_ReturnsAllRegisteredTenants()
    {
        _store.All.Count().ShouldBe(2);
    }
}
