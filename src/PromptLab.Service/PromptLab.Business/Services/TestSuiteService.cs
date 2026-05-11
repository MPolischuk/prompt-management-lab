using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.TestCases;
using PromptLab.Entities.TestSuites;

namespace PromptLab.Business.Services;

public class TestSuiteService(ITestSuiteRepository suiteRepository, ITestCaseRepository caseRepository) : ITestSuiteService
{
    public Task<IReadOnlyList<TestSuite>> GetByPromptIdAsync(Guid promptId, CancellationToken cancellationToken)
    {
        return suiteRepository.GetByPromptIdAsync(promptId, cancellationToken);
    }

    public async Task<TestSuiteDetail?> GetByIdWithCasesAsync(Guid id, CancellationToken cancellationToken)
    {
        var suite = await suiteRepository.GetByIdAsync(id, cancellationToken);
        if (suite is null)
        {
            return null;
        }

        var cases = await caseRepository.GetBySuiteIdAsync(id, cancellationToken);
        return new TestSuiteDetail { Suite = suite, Cases = cases };
    }

    public Task<OperationResult> CreateAsync(CreateTestSuiteRequest request, CancellationToken cancellationToken)
    {
        return suiteRepository.CreateAsync(request, cancellationToken);
    }

    public Task<OperationResult> UpdateAsync(Guid id, UpdateTestSuiteRequest request, CancellationToken cancellationToken)
    {
        return suiteRepository.UpdateAsync(id, request, cancellationToken);
    }

    public Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        return suiteRepository.DeleteAsync(id, cancellationToken);
    }
}
