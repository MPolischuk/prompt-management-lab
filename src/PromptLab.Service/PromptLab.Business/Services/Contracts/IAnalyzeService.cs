using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;

namespace PromptLab.Business.Services.Contracts;

/// <summary>
/// Expone casos de uso para ejecucion y consulta de analisis.
/// </summary>
public interface IAnalyzeService
{
    /// <summary>Ejecuta un analisis para un prompt y retorna el resultado operativo.</summary>
    Task<OperationResult> AnalyzeAsync(AnalyzeRequest request, CancellationToken cancellationToken);
    /// <summary>Obtiene una corrida de analisis por identificador.</summary>
    Task<AnalyzeRun?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    /// <summary>Lista proveedores de analisis disponibles y su estado.</summary>
    Task<IReadOnlyCollection<AnalyzeProvider>> GetProvidersAsync(CancellationToken cancellationToken);
    /// <summary>Lista modelos de analisis habilitados.</summary>
    Task<IReadOnlyCollection<AiModel>> GetModelsAsync(CancellationToken cancellationToken);
}
