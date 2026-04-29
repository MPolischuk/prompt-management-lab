using PromptLab.Entities.Common;
using PromptLab.Entities.Tags;

namespace PromptLab.Business.Services;

/// <summary>
/// Expone casos de uso para gestion de tags.
/// </summary>
public interface ITagService
{
    /// <summary>Obtiene todos los tags.</summary>
    Task<IReadOnlyCollection<Tag>> GetAllAsync(CancellationToken cancellationToken);
    /// <summary>Busca tags por texto.</summary>
    Task<IReadOnlyCollection<Tag>> SearchAsync(string? query, CancellationToken cancellationToken);
    /// <summary>Crea un nuevo tag.</summary>
    Task<OperationResult> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken);
}
