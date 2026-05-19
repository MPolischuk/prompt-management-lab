namespace PromptLab.Entities.TestResults;

public sealed class CreateTestResultRequest
{
    public Guid RunId { get; init; }
    public Guid CaseId { get; init; }
    public required string ActualOutput { get; init; }
    public bool Passed { get; init; }
    public decimal Score { get; init; }
    public int LatencyMs { get; init; }
    public string? Error { get; init; }
}
