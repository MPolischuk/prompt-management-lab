namespace PromptLab.Entities.TestRuns;

public sealed class UpdateTestRunRequest
{
    public required string Status { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}
