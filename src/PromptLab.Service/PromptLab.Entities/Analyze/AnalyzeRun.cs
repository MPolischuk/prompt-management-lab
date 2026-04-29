namespace PromptLab.Entities.Analyze;

public class AnalyzeRun
{
    public Guid Id { get; init; }
    public Guid PromptId { get; init; }
    public string? PromptTitle { get; init; }
    public required string Provider { get; init; }
    public string? Input { get; init; }
    public string? Output { get; init; }
    public required string Status { get; init; }
    public string? ErrorMessage { get; init; }
    public int? LatencyMs { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}
