using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.TestCases;

namespace PromptLab.Business.Services;

public class TestCaseService(ITestCaseRepository repository) : ITestCaseService
{
    public Task<IReadOnlyList<TestCase>> GetBySuiteIdAsync(Guid suiteId, CancellationToken cancellationToken)
    {
        return repository.GetBySuiteIdAsync(suiteId, cancellationToken);
    }

    public Task<OperationResult> CreateAsync(CreateTestCaseRequest request, CancellationToken cancellationToken)
    {
        return repository.CreateAsync(request, cancellationToken);
    }

    public Task<OperationResult> UpdateAsync(Guid id, UpdateTestCaseRequest request, CancellationToken cancellationToken)
    {
        return repository.UpdateAsync(id, request, cancellationToken);
    }

    public Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return repository.DeleteAsync(id, cancellationToken);
    }
}
