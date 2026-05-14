using FluentAssertions;
using PromptLab.Business.Ai.OpenAi;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Integrations.Tests.Ai.OpenAi;

public class GptAiProviderRequestMockTests
{
    [Fact]
    public async Task AnalyzeAsync_ReturnsOutputWithMockPrefix()
    {
        var sut = new GptAiProviderRequestMock();
        var prompt = new Prompt { Id = Guid.NewGuid(), Title = "MyPrompt", Content = "Body", IsActive = true };
        var request = new AnalyzeExecutionRequest
        {
            Provider = "openai",
            ModelId = "gpt-test",
            Input = "user in",
            EffectiveSettings = new GenerationSettings()
        };

        var result = await sut.AnalyzeAsync(prompt, request, CancellationToken.None);

        result.Status.Should().Be("Completed");
        result.Output.Should().StartWith("[MOCK:openai]");
        result.Output.Should().Contain("gpt-test");
        result.Output.Should().Contain("MyPrompt");
        result.LatencyMs.Should().BeGreaterThanOrEqualTo(50).And.BeLessThan(221);
    }

    [Fact]
    public async Task AnalyzeAsync_WhenCanceled_PropagatesCancellation()
    {
        var sut = new GptAiProviderRequestMock();
        var prompt = new Prompt { Id = Guid.NewGuid(), Title = "T", Content = "C", IsActive = true };
        var request = new AnalyzeExecutionRequest
        {
            Provider = "openai",
            ModelId = "m",
            EffectiveSettings = new GenerationSettings()
        };
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await sut.AnalyzeAsync(prompt, request, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
