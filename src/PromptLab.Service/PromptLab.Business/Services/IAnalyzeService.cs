using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;

namespace PromptLab.Business.Services;

public interface IAnalyzeService
{
    Task<OperationResult> AnalyzeAsync(AnalyzeRequest request, CancellationToken cancellationToken);
    Task<AnalyzeRun?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<AnalyzeProvider>> GetProvidersAsync(CancellationToken cancellationToken);
}
