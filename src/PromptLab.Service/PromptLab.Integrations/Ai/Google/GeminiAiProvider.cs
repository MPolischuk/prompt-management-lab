using PromptLab.Business.Ai.Contracts;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai.Google;

public class GeminiAiProvider(IGeminiAiProviderRequestClient client) : IAiProvider
{
    public string Name => "google";

    public Task<AnalyzeExecutionResult> AnalyzeAsync(
        Prompt prompt,
        AnalyzeExecutionRequest request,
        CancellationToken cancellationToken)
    {
        return client.AnalyzeAsync(prompt, request, cancellationToken);
    }
}
