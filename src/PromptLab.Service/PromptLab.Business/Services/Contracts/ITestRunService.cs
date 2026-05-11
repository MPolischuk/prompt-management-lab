using PromptLab.Entities.Common;
using PromptLab.Entities.TestResults;
using PromptLab.Entities.TestRuns;

namespace PromptLab.Business.Services.Contracts;

public interface ITestRunService
{
    Task<IReadOnlyList<TestRun>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<TestRun>> GetBySuiteIdAsync(Guid suiteId, CancellationToken cancellationToken);
    Task<TestRunDetail?> GetByIdWithResultsAsync(Guid id, CancellationToken cancellationToken);
    Task<OperationResult> CreateAsync(CreateTestRunRequest request, CancellationToken cancellationToken);
    Task<OperationResult> UpdateAsync(Guid id, UpdateTestRunRequest request, CancellationToken cancellationToken);
    Task<OperationResult> CreateResultAsync(CreateTestResultRequest request, CancellationToken cancellationToken);
}
