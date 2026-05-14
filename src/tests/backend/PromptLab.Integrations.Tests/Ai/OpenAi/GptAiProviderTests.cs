using FluentAssertions;
using Moq;
using PromptLab.Business.Ai.Contracts;
using PromptLab.Business.Ai.OpenAi;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Integrations.Tests.Ai.OpenAi;

public class GptAiProviderTests
{
    [Fact]
    public void Name_IsOpenai()
    {
        var client = new Mock<IGptAiProviderRequestClient>();
        var sut = new GptAiProvider(client.Object);

        sut.Name.Should().Be("openai");
    }

    [Fact]
    public async Task AnalyzeAsync_DelegatesToRequestClient()
    {
        var prompt = new Prompt { Id = Guid.NewGuid(), Title = "T", Content = "C", IsActive = true };
        var request = new AnalyzeExecutionRequest
        {
            Provider = "openai",
            ModelId = "m",
            Input = "i",
            EffectiveSettings = new GenerationSettings()
        };
        var expected = new AnalyzeExecutionResult { Status = "Completed", Output = "x", LatencyMs = 0 };
        var client = new Mock<IGptAiProviderRequestClient>();
        client.Setup(c => c.AnalyzeAsync(prompt, request, It.IsAny<CancellationToken>())).ReturnsAsync(expected);
        var sut = new GptAiProvider(client.Object);

        var result = await sut.AnalyzeAsync(prompt, request, CancellationToken.None);

        result.Should().BeSameAs(expected);
        client.Verify(c => c.AnalyzeAsync(prompt, request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
