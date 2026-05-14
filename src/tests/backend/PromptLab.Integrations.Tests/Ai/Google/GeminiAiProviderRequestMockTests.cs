using FluentAssertions;
using PromptLab.Business.Ai.Google;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Integrations.Tests.Ai.Google;

public class GeminiAiProviderRequestMockTests
{
    [Fact]
    public async Task AnalyzeAsync_ReturnsOutputWithMockPrefix()
    {
        var sut = new GeminiAiProviderRequestMock();
        var prompt = new Prompt { Id = Guid.NewGuid(), Title = "P", Content = "B", IsActive = true };
        var request = new AnalyzeExecutionRequest
        {
            Provider = "google",
            ModelId = "gemini",
            Input = "i",
            EffectiveSettings = new GenerationSettings()
        };

        var result = await sut.AnalyzeAsync(prompt, request, CancellationToken.None);

        result.Output.Should().StartWith("[MOCK:google]");
        result.Status.Should().Be("Completed");
    }

    [Fact]
    public async Task AnalyzeAsync_WhenCanceled_PropagatesCancellation()
    {
        var sut = new GeminiAiProviderRequestMock();
        var prompt = new Prompt { Id = Guid.NewGuid(), Title = "T", Content = "C", IsActive = true };
        var request = new AnalyzeExecutionRequest
        {
            Provider = "google",
            ModelId = "m",
            EffectiveSettings = new GenerationSettings()
        };
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await sut.AnalyzeAsync(prompt, request, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
