namespace PromptLab.Entities.Analyze;

public class AnalyzeRequest
{
    public Guid PromptId { get; init; }
    public string? Provider { get; init; }
    public string? ModelId { get; init; }
    public string? Input { get; init; }
    public GenerationSettings? Settings { get; init; }
}
