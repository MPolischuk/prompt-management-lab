using PromptLab.Entities.Common;
using PromptLab.Entities.Prompts;

namespace PromptLab.Entities.Contracts;

/// <summary>
/// Define operaciones de persistencia para prompts.
/// </summary>
public interface IPromptRepository
{
    /// <summary>Crea un prompt.</summary>
    Task<OperationResult> CreateAsync(UpsertPromptRequest request, CancellationToken cancellationToken);
    /// <summary>Actualiza un prompt existente.</summary>
    Task<OperationResult> UpdateAsync(Guid id, UpsertPromptRequest request, CancellationToken cancellationToken);
    /// <summary>Elimina un prompt por identificador.</summary>
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken);
    /// <summary>Obtiene un prompt por identificador.</summary>
    Task<Prompt?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    /// <summary>Busca prompts con filtros y paginacion.</summary>
    Task<PagedResponse<Prompt>> SearchAsync(PromptSearchRequest request, CancellationToken cancellationToken);
    /// <summary>Asocia tags a un prompt.</summary>
    Task<OperationResult> SetTagsAsync(Guid promptId, IReadOnlyCollection<Guid> tagIds, CancellationToken cancellationToken);
}
