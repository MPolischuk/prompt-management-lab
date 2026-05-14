using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PromptLab.Business.Ai;
using PromptLab.Business.Ai.Anthropic;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Integrations.Tests.Ai.Anthropic;

public class ClaudeAiProviderRequestMockTests
{
    [Fact]
    public async Task AnalyzeAsync_ReturnsOutputWithMockPrefix()
    {
        var sut = new ClaudeAiProviderRequestMock();
        var prompt = new Prompt { Id = Guid.NewGuid(), Title = "P", Content = "Body", IsActive = true };
        var request = new AnalyzeExecutionRequest
        {
            Provider = "anthropic",
            ModelId = "claude",
            Input = "in",
            EffectiveSettings = new GenerationSettings()
        };

        var result = await sut.AnalyzeAsync(prompt, request, CancellationToken.None);

        result.Output.Should().StartWith("[MOCK:anthropic]");
        result.Status.Should().Be("Completed");
    }

    [Fact]
    public async Task AnalyzeAsync_WhenCanceled_PropagatesCancellation()
    {
        var sut = new ClaudeAiProviderRequestMock();
        var prompt = new Prompt { Id = Guid.NewGuid(), Title = "T", Content = "C", IsActive = true };
        var request = new AnalyzeExecutionRequest
        {
            Provider = "anthropic",
            ModelId = "m",
            EffectiveSettings = new GenerationSettings()
        };
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await sut.AnalyzeAsync(prompt, request, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
