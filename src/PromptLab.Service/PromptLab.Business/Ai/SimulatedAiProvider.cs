using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai;

public class SimulatedAiProvider : IAiProvider
{
    public string Name => "simulated";

    public Task<AnalyzeExecutionResult> AnalyzeAsync(Prompt prompt, AnalyzeExecutionRequest request, CancellationToken cancellationToken)
    {
        var output =
            $"[SIMULATED] Provider={request.Provider} Model={request.ModelId} Prompt='{prompt.Title}' input: {request.Input ?? "(sin input)"}";
        return Task.FromResult(new AnalyzeExecutionResult
        {
            Output = output,
            LatencyMs = Random.Shared.Next(80, 350),
            Status = "Completed"
        });
    }
}
