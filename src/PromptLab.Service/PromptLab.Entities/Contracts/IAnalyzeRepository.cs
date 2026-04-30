using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;

namespace PromptLab.Entities.Contracts;

/// <summary>
/// Define operaciones de persistencia para ejecuciones de analisis.
/// </summary>
public interface IAnalyzeRepository
{
    /// <summary>Persiste una ejecucion de analisis.</summary>
    Task<OperationResult> CreateRunAsync(AnalyzeRun run, CancellationToken cancellationToken);
    /// <summary>Obtiene una ejecucion por identificador.</summary>
    Task<AnalyzeRun?> GetRunByIdAsync(Guid id, CancellationToken cancellationToken);
}
