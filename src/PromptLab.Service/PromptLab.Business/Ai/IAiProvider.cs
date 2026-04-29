using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai;

public interface IAiProvider
{
    string Name { get; }
    Task<AnalyzeExecutionResult> AnalyzeAsync(Prompt prompt, AnalyzeRequest request, CancellationToken cancellationToken);
}
