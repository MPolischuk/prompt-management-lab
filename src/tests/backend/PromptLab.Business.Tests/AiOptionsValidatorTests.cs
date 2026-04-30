using FluentAssertions;
using PromptLab.Business.Configuration;

namespace PromptLab.Business.Tests;

public class AiOptionsValidatorTests
{
    private readonly AiOptionsValidator _validator = new();

    [Fact]
    public void Validate_WhenDefaultProviderMissingInEnabledProviders_ReturnsFailure()
    {
        var options = new AiOptions
        {
            DefaultProvider = "openai",
            EnabledProviders = ["simulated"]
        };

        var result = _validator.Validate(name: null, options);

        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("DefaultProvider", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validate_WhenModelUsesDisabledProvider_ReturnsFailure()
    {
        var options = new AiOptions
        {
            DefaultProvider = "simulated",
            EnabledProviders = ["simulated"],
            Models =
            [
                new AiModelOption { Id = "gpt-5.5", Provider = "openai" }
            ]
        };

        var result = _validator.Validate(name: null, options);

        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("Ai:Models provider 'openai'", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Validate_WhenConfigurationIsConsistent_ReturnsSuccess()
    {
        var options = new AiOptions
        {
            DefaultProvider = "simulated",
            EnabledProviders = ["simulated", "openai"],
            Models =
            [
                new AiModelOption { Id = "simulated-default", Provider = "simulated" },
                new AiModelOption { Id = "gpt-5.5", Provider = "openai" }
            ]
        };

        var result = _validator.Validate(name: null, options);

        result.Succeeded.Should().BeTrue();
    }
}
