using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using PromptLab.Business.Ai;
using PromptLab.Business.Ai.Contracts;
using PromptLab.Business.Configuration;

namespace PromptLab.Business.Tests;

public class AiProviderFactoryTests
{
    private static AiOptions OptionsWithDefault(string defaultProvider = "openai") =>
        new()
        {
            DefaultProvider = defaultProvider,
            EnabledProviders = [defaultProvider],
            Models = []
        };

    [Fact]
    public void GetConfiguredProviderNames_ReturnsNamesOfAllRegistered()
    {
        var p1 = new Mock<IAiProvider>();
        p1.SetupGet(p => p.Name).Returns("openai");
        var p2 = new Mock<IAiProvider>();
        p2.SetupGet(p => p.Name).Returns("anthropic");
        var factory = new AiProviderFactory([p1.Object, p2.Object], Options.Create(OptionsWithDefault()));

        var names = factory.GetConfiguredProviderNames();

        names.Should().BeEquivalentTo(["openai", "anthropic"], options => options.WithStrictOrdering());
    }

    [Fact]
    public void Resolve_WhenProviderExists_ReturnsCorrectProvider()
    {
        var openAi = new Mock<IAiProvider>();
        openAi.SetupGet(p => p.Name).Returns("openai");
        var factory = new AiProviderFactory([openAi.Object], Options.Create(OptionsWithDefault("openai")));

        var resolved = factory.Resolve("openai");

        resolved.Should().BeSameAs(openAi.Object);
    }

    [Fact]
    public void Resolve_WhenProviderNotFound_ReturnsNull()
    {
        var factory = new AiProviderFactory([], Options.Create(OptionsWithDefault("openai")));

        var resolved = factory.Resolve("missing");

        resolved.Should().BeNull();
    }

    [Fact]
    public void Resolve_WhenProviderNameIsNull_UsesDefaultFromOptions()
    {
        var openAi = new Mock<IAiProvider>();
        openAi.SetupGet(p => p.Name).Returns("openai");
        var factory = new AiProviderFactory([openAi.Object], Options.Create(OptionsWithDefault("openai")));

        var resolved = factory.Resolve(null);

        resolved.Should().BeSameAs(openAi.Object);
    }

    [Fact]
    public void Resolve_IsCaseInsensitive()
    {
        var openAi = new Mock<IAiProvider>();
        openAi.SetupGet(p => p.Name).Returns("OpenAI");
        var factory = new AiProviderFactory([openAi.Object], Options.Create(OptionsWithDefault("openai")));

        var resolved = factory.Resolve("OPENAI");

        resolved.Should().BeSameAs(openAi.Object);
    }
}
