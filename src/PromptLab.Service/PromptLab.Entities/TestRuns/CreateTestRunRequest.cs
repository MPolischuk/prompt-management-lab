namespace PromptLab.Entities.TestRuns;

public sealed class CreateTestRunRequest
{
    public Guid SuiteId { get; init; }
    public Guid PromptId { get; init; }
    public int PromptVersion { get; init; }
    public required string Model { get; init; }
    public decimal Temperature { get; init; }
    public int? MaxTokens { get; init; }
    public required string Status { get; init; }
}
