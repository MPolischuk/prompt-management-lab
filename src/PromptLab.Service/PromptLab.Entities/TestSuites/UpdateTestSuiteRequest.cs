namespace PromptLab.Entities.TestSuites;

public sealed class UpdateTestSuiteRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
