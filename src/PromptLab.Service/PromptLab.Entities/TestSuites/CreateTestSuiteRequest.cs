namespace PromptLab.Entities.TestSuites;

public sealed class CreateTestSuiteRequest
{
    public Guid PromptId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}
