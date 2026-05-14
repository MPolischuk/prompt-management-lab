using FluentAssertions;
using PromptLab.Business.Ai;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Integrations.Tests.Ai;

public class AiPromptComposerTests
{
    private static AnalyzeExecutionRequest Request(string? input) =>
        new()
        {
            Provider = "openai",
            ModelId = "m",
            Input = input,
            EffectiveSettings = new GenerationSettings()
        };

    [Fact]
    public void BuildUserFacingText_WhenInputNullOrEmpty_ReturnsPromptContentOnly()
    {
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "t",
            Content = "Instrucciones base",
            Version = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Tags = [],
            TagSummaries = []
        };

        var text = AiPromptComposer.BuildUserFacingText(prompt, Request(null));

        text.Should().Be("Instrucciones base");
    }

    [Fact]
    public void BuildUserFacingText_WhenInputWhitespace_ReturnsPromptContentOnly()
    {
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "t",
            Content = "Solo esto",
            Version = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Tags = [],
            TagSummaries = []
        };

        var text = AiPromptComposer.BuildUserFacingText(prompt, Request("   \t"));

        text.Should().Be("Solo esto");
    }

    [Fact]
    public void BuildUserFacingText_WhenInputPresent_AppendsUserBlock()
    {
        var prompt = new Prompt
        {
            Id = Guid.NewGuid(),
            Title = "t",
            Content = "Contexto",
            Version = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Tags = [],
            TagSummaries = []
        };

        var text = AiPromptComposer.BuildUserFacingText(prompt, Request("  hola  "));

        text.Should().Be("Contexto\n\n--- User input ---\nhola");
    }
}
