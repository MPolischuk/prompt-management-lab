using PromptLab.Entities.Common;
using PromptLab.Entities.TestResults;
using PromptLab.Entities.TestRuns;

namespace PromptLab.Entities.Contracts;

public interface ITestRunRepository
{
    Task<IReadOnlyList<TestRun>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<TestRun>> GetBySuiteIdAsync(Guid suiteId, CancellationToken cancellationToken);
    Task<TestRun?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<OperationResult> CreateAsync(CreateTestRunRequest request, CancellationToken cancellationToken);
    Task<OperationResult> UpdateAsync(Guid id, UpdateTestRunRequest request, CancellationToken cancellationToken);
    Task<OperationResult> CreateResultAsync(CreateTestResultRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<TestResult>> GetResultsByRunIdAsync(Guid runId, CancellationToken cancellationToken);
}
