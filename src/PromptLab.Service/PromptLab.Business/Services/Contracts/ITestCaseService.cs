using PromptLab.Entities.Common;
using PromptLab.Entities.TestCases;

namespace PromptLab.Business.Services.Contracts;

public interface ITestCaseService
{
    Task<IReadOnlyList<TestCase>> GetBySuiteIdAsync(Guid suiteId, CancellationToken cancellationToken);
    Task<OperationResult> CreateAsync(CreateTestCaseRequest request, CancellationToken cancellationToken);
    Task<OperationResult> UpdateAsync(Guid id, UpdateTestCaseRequest request, CancellationToken cancellationToken);
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
