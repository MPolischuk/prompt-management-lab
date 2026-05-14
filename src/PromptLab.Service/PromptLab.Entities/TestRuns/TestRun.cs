namespace PromptLab.Entities.TestRuns;

public sealed class TestRun
{
    public Guid Id { get; init; }
    public Guid SuiteId { get; init; }
    public Guid PromptId { get; init; }
    public int PromptVersion { get; init; }
    public required string Model { get; init; }
    public decimal Temperature { get; init; }
    public int? MaxTokens { get; init; }
    public required string Status { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? PromptTitle { get; init; }
    public string? SuiteName { get; init; }
}
