using FluentAssertions;
using Microsoft.Extensions.Options;
using PromptLab.Data.Configuration;
using PromptLab.Data.Infrastructure;

namespace PromptLab.Data.Tests.Infrastructure;

public class SqlConnectionFactoryTests
{
    [Fact]
    public void CreateConnection_ReturnsSqlConnectionWithConfiguredString()
    {
        const string cs = "Server=test;Database=promptlab;Trusted_Connection=True;";
        var factory = new SqlConnectionFactory(Options.Create(new SqlOptions { ConnectionString = cs }));

        using var conn = factory.CreateConnection();

        conn.Should().NotBeNull();
        conn.ConnectionString.Should().Be(cs);
    }
}
