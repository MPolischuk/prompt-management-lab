namespace PromptLab.Entities.TestCases;

public sealed class TestCase
{
    public Guid Id { get; init; }
    public Guid SuiteId { get; init; }
    public required string Name { get; init; }
    /// <summary>JSON object de variables de entrada.</summary>
    public required string InputVariables { get; init; }
    public string? ExpectedOutput { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
