using PromptLab.Business.Ai.Contracts;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai.OpenAi;

public class GptAiProvider(IGptAiProviderRequestClient client) : IAiProvider
{
    public string Name => "openai";

    public Task<AnalyzeExecutionResult> AnalyzeAsync(
        Prompt prompt,
        AnalyzeExecutionRequest request,
        CancellationToken cancellationToken)
    {
        return client.AnalyzeAsync(prompt, request, cancellationToken);
    }
}
