using PromptLab.Entities.TestCases;

namespace PromptLab.Entities.TestSuites;

public sealed class TestSuiteDetail
{
    public required TestSuite Suite { get; init; }
    public IReadOnlyList<TestCase> Cases { get; init; } = [];
}
