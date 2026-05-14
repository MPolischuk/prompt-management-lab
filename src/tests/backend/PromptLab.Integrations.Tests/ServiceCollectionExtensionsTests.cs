using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PromptLab.Business.Ai.Contracts;
using PromptLab.Business.Configuration;
using PromptLab.Integrations;

namespace PromptLab.Integrations.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPromptLabIntegrations_WhenAllVendorsEnabled_RegistersThreeProviders()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Ai:DefaultProvider"] = "openai",
                ["Ai:EnabledProviders:0"] = "openai",
                ["Ai:Providers:OpenAi:Enabled"] = "true",
                ["Ai:Providers:OpenAi:Mock"] = "true",
                ["Ai:Providers:Anthropic:Enabled"] = "true",
                ["Ai:Providers:Anthropic:Mock"] = "true",
                ["Ai:Providers:Google:Enabled"] = "true",
                ["Ai:Providers:Google:Mock"] = "true"
            })
            .Build();

        services.Configure<AiOptions>(config.GetSection(AiOptions.SectionName));
        services.AddPromptLabIntegrations(config);

        using var sp = services.BuildServiceProvider();
        var providers = sp.GetServices<IAiProvider>().ToList();

        providers.Should().HaveCount(3);
    }
}
