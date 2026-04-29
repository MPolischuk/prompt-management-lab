namespace PromptLab.Entities.Analyze;

public class AnalyzeRequest
{
    public Guid PromptId { get; init; }
    public required string Provider { get; init; }
    public string? Input { get; init; }
}
