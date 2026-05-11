using PromptLab.Entities.TestResults;

namespace PromptLab.Entities.TestRuns;

public sealed class TestRunDetail
{
    public required TestRun Run { get; init; }
    public IReadOnlyList<TestResult> Results { get; init; } = [];
}
