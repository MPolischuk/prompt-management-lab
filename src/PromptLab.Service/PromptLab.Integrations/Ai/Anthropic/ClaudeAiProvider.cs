using PromptLab.Business.Ai.Contracts;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai.Anthropic;

public class ClaudeAiProvider(IClaudeAiProviderRequestClient client) : IAiProvider
{
    public string Name => "anthropic";

    public Task<AnalyzeExecutionResult> AnalyzeAsync(
        Prompt prompt,
        AnalyzeExecutionRequest request,
        CancellationToken cancellationToken)
    {
        return client.AnalyzeAsync(prompt, request, cancellationToken);
    }
}
