using PromptLab.Entities.Common;
using PromptLab.Entities.TestSuites;

namespace PromptLab.Business.Services.Contracts;

public interface ITestSuiteService
{
    Task<IReadOnlyList<TestSuite>> GetByPromptIdAsync(Guid promptId, CancellationToken cancellationToken);
    Task<TestSuiteDetail?> GetByIdWithCasesAsync(Guid id, CancellationToken cancellationToken);
    Task<OperationResult> CreateAsync(CreateTestSuiteRequest request, CancellationToken cancellationToken);
    Task<OperationResult> UpdateAsync(Guid id, UpdateTestSuiteRequest request, CancellationToken cancellationToken);
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
