using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai;

public class SimulatedAiProvider : IAiProvider
{
    public string Name => "simulated";

    public Task<AnalyzeExecutionResult> AnalyzeAsync(Prompt prompt, AnalyzeRequest request, CancellationToken cancellationToken)
    {
        var output = $"[SIMULATED] Prompt '{prompt.Title}' ejecutado con input: {request.Input ?? "(sin input)"}";
        return Task.FromResult(new AnalyzeExecutionResult
        {
            Output = output,
            LatencyMs = Random.Shared.Next(80, 350),
            Status = "Completed"
        });
    }
}
