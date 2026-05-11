using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.TestResults;
using PromptLab.Entities.TestRuns;

namespace PromptLab.Business.Services;

public class TestRunService(ITestRunRepository repository) : ITestRunService
{
    public Task<IReadOnlyList<TestRun>> GetAllAsync(CancellationToken cancellationToken)
    {
        return repository.GetAllAsync(cancellationToken);
    }

    public Task<IReadOnlyList<TestRun>> GetBySuiteIdAsync(Guid suiteId, CancellationToken cancellationToken)
    {
        return repository.GetBySuiteIdAsync(suiteId, cancellationToken);
    }

    public async Task<TestRunDetail?> GetByIdWithResultsAsync(Guid id, CancellationToken cancellationToken)
    {
        var run = await repository.GetByIdAsync(id, cancellationToken);
        if (run is null)
        {
            return null;
        }

        var results = await repository.GetResultsByRunIdAsync(id, cancellationToken);
        return new TestRunDetail { Run = run, Results = results };
    }

    public Task<OperationResult> CreateAsync(CreateTestRunRequest request, CancellationToken cancellationToken)
    {
        return repository.CreateAsync(request, cancellationToken);
    }

    public Task<OperationResult> UpdateAsync(Guid id, UpdateTestRunRequest request, CancellationToken cancellationToken)
    {
        return repository.UpdateAsync(id, request, cancellationToken);
    }

    public Task<OperationResult> CreateResultAsync(CreateTestResultRequest request, CancellationToken cancellationToken)
    {
        return repository.CreateResultAsync(request, cancellationToken);
    }
}
