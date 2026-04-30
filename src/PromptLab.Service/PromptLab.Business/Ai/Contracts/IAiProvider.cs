using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai.Contracts;

public interface IAiProvider
{
    string Name { get; }
    Task<AnalyzeExecutionResult> AnalyzeAsync(Prompt prompt, AnalyzeExecutionRequest request, CancellationToken cancellationToken);
}
