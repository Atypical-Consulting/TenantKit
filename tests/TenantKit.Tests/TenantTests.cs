using FluentAssertions;
using TenantKit.Core;
using Xunit;

namespace TenantKit.Tests;

public sealed class TenantTests
{
    [Fact]
    public void Create_SetsIdAndName()
    {
        var tenant = Tenant.Create("acme", "Acme Corp");

        tenant.Id.Should().Be("acme");
        tenant.Name.Should().Be("Acme Corp");
    }

    [Fact]
    public void Create_WithMetadata_StoresKeyValues()
    {
        var tenant = Tenant.Create("acme", "Acme Corp", new Dictionary<string, string>
        {
            ["plan"] = "enterprise",
            ["region"] = "eu-west-1"
        });

        ((ITenant)tenant).Metadata["plan"].Should().Be("enterprise");
        ((ITenant)tenant).Metadata["region"].Should().Be("eu-west-1");
    }

    [Fact]
    public void Create_NoMetadata_ReturnsEmptyDictionary()
    {
        var tenant = Tenant.Create("free", "Free User");

        ((ITenant)tenant).Metadata.Should().BeEmpty();
    }

    [Fact]
    public void Records_WithSameValues_AreEqual()
    {
        var a = Tenant.Create("id1", "Name");
        var b = Tenant.Create("id1", "Name");

        a.Should().Be(b);
    }
}
