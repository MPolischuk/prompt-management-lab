using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai.Google;

public interface IGeminiAiProviderRequestClient
{
    Task<AnalyzeExecutionResult> AnalyzeAsync(
        Prompt prompt,
        AnalyzeExecutionRequest request,
        CancellationToken cancellationToken);
}
