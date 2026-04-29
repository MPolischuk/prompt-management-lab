using PromptLab.Entities.Common;
using PromptLab.Entities.Tags;

namespace PromptLab.Business.Repositories;

/// <summary>
/// Define operaciones de persistencia para tags.
/// </summary>
public interface ITagRepository
{
    /// <summary>Obtiene todos los tags disponibles.</summary>
    Task<IReadOnlyCollection<Tag>> GetAllAsync(CancellationToken cancellationToken);
    /// <summary>Busca tags por texto libre.</summary>
    Task<IReadOnlyCollection<Tag>> SearchAsync(string? query, CancellationToken cancellationToken);
    /// <summary>Crea un nuevo tag.</summary>
    Task<OperationResult> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken);
}
