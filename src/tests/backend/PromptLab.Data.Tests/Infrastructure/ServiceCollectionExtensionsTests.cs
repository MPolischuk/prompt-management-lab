using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromptLab.Data;
using PromptLab.Data.Infrastructure;
using PromptLab.Data.Repositories;
using PromptLab.Data.Tests.Infrastructure;
using PromptLab.Entities.Contracts;

namespace PromptLab.Data.Tests.Infrastructure;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPromptLabData_RegistersRepositoriesAndFactory()
    {
        RepoDbTestInitializer.EnsureSqlServer();
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Sql:ConnectionString"] = "Server=x;" })
            .Build();

        services.AddPromptLabData(configuration);
        using var sp = services.BuildServiceProvider();

        sp.GetRequiredService<IDbConnectionFactory>().Should().BeOfType<SqlConnectionFactory>();
        sp.GetRequiredService<IPromptRepository>().Should().BeOfType<PromptRepository>();
        sp.GetRequiredService<IPromptVersionRepository>().Should().BeOfType<PromptVersionRepository>();
        sp.GetRequiredService<ITestSuiteRepository>().Should().BeOfType<TestSuiteRepository>();
        sp.GetRequiredService<ITestCaseRepository>().Should().BeOfType<TestCaseRepository>();
        sp.GetRequiredService<ITestRunRepository>().Should().BeOfType<TestRunRepository>();
        sp.GetRequiredService<ITagRepository>().Should().BeOfType<TagRepository>();
        sp.GetRequiredService<IAnalyzeRepository>().Should().BeOfType<AnalyzeRepository>();
    }
}
