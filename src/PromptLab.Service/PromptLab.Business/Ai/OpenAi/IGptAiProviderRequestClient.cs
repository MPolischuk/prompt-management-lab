using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai.OpenAi;

public interface IGptAiProviderRequestClient
{
    Task<AnalyzeExecutionResult> AnalyzeAsync(
        Prompt prompt,
        AnalyzeExecutionRequest request,
        CancellationToken cancellationToken);
}
