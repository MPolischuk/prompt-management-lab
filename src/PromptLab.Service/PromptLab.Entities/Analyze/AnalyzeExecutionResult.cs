namespace PromptLab.Entities.Analyze;

public class AnalyzeExecutionResult
{
    public required string Output { get; init; }
    public required int LatencyMs { get; init; }
    public required string Status { get; init; }
    public string? ErrorMessage { get; init; }
}
