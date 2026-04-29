namespace PromptLab.Entities.Analyze;

public class AnalyzeExecutionRequest
{
    public required string Provider { get; init; }
    public required string ModelId { get; init; }
    public string? Input { get; init; }
    public required GenerationSettings EffectiveSettings { get; init; }
}
