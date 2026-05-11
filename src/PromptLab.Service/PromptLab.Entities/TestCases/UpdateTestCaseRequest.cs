namespace PromptLab.Entities.TestCases;

public sealed class UpdateTestCaseRequest
{
    public required string Name { get; init; }
    public required string InputVariables { get; init; }
    public string? ExpectedOutput { get; init; }
}
