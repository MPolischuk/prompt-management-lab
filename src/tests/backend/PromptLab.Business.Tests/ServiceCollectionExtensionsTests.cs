using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PromptLab.Business;
using PromptLab.Business.Ai;
using PromptLab.Business.Ai.Contracts;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Contracts;

namespace PromptLab.Business.Tests;

public class ServiceCollectionExtensionsTests
{
    private static IConfiguration BuildMinimalConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cache:PromptSearchTtlSeconds"] = "60",
                ["Ai:DefaultProvider"] = "openai",
                ["Ai:EnabledProviders:0"] = "openai"
            })
            .Build();

    [Fact]
    public void AddPromptLabBusiness_RegistersExpectedServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Mock.Of<IPromptRepository>());
        services.AddSingleton(Mock.Of<IPromptVersionRepository>());
        services.AddSingleton(Mock.Of<ITagRepository>());
        services.AddSingleton(Mock.Of<ITestSuiteRepository>());
        services.AddSingleton(Mock.Of<ITestCaseRepository>());
        services.AddSingleton(Mock.Of<ITestRunRepository>());
        services.AddSingleton(Mock.Of<IAnalyzeRepository>());
        var aiProvider = new Mock<IAiProvider>();
        aiProvider.SetupGet(p => p.Name).Returns("openai");
        services.AddSingleton<IAiProvider>(aiProvider.Object);

        services.AddPromptLabBusiness(BuildMinimalConfiguration());

        using var sp = services.BuildServiceProvider();

        sp.GetRequiredService<IPromptService>().Should().NotBeNull();
        sp.GetRequiredService<ITagService>().Should().NotBeNull();
        sp.GetRequiredService<ITestSuiteService>().Should().NotBeNull();
        sp.GetRequiredService<ITestCaseService>().Should().NotBeNull();
        sp.GetRequiredService<ITestRunService>().Should().NotBeNull();
        sp.GetRequiredService<IAnalyzeService>().Should().NotBeNull();
        sp.GetRequiredService<AiProviderFactory>().Should().NotBeNull();
    }
}
