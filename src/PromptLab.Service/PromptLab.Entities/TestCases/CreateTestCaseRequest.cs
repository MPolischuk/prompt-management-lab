namespace PromptLab.Entities.TestCases;

public sealed class CreateTestCaseRequest
{
    public Guid SuiteId { get; init; }
    public required string Name { get; init; }
    public required string InputVariables { get; init; }
    public string? ExpectedOutput { get; init; }
}
