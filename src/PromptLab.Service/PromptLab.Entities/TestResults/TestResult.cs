namespace PromptLab.Entities.TestResults;

public sealed class TestResult
{
    public Guid Id { get; init; }
    public Guid RunId { get; init; }
    public Guid CaseId { get; init; }
    public required string ActualOutput { get; init; }
    public bool Passed { get; init; }
    public decimal Score { get; init; }
    public int LatencyMs { get; init; }
    public string? Error { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CaseName { get; init; }
    public string? InputVariables { get; init; }
    public string? ExpectedOutput { get; init; }
}
