using PromptLab.Business.Ai.Contracts;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai.Anthropic;

public class ClaudeAiProviderRequestMock : IClaudeAiProviderRequestClient
{
    public Task<AnalyzeExecutionResult> AnalyzeAsync(
        Prompt prompt,
        AnalyzeExecutionRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var text = AiPromptComposer.BuildUserFacingText(prompt, request);
        var output =
            $"[MOCK:anthropic] Model={request.ModelId} Prompt='{prompt.Title}' — {text}";
        return Task.FromResult(new AnalyzeExecutionResult
        {
            Output = output,
            LatencyMs = Random.Shared.Next(50, 220),
            Status = "Completed"
        });
    }
}
