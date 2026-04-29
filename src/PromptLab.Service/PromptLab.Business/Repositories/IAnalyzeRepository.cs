using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;

namespace PromptLab.Business.Repositories;

public interface IAnalyzeRepository
{
    Task<OperationResult> CreateRunAsync(AnalyzeRun run, CancellationToken cancellationToken);
    Task<AnalyzeRun?> GetRunByIdAsync(Guid id, CancellationToken cancellationToken);
}
