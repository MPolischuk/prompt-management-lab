using PromptLab.Entities.Common;

namespace PromptLab.Entities.Prompts;

public class PromptSearchRequest : PagedRequest
{
    public string? Query { get; init; }
    public string? Category { get; init; }
    public string? Language { get; init; }
    /// <summary>Si es null, la busqueda solo incluye prompts activos. Use false para listar solo dados de baja.</summary>
    public bool? IsActive { get; init; }
    public Guid? TagId { get; init; }
    public DateTime? CreatedFrom { get; init; }
    public DateTime? CreatedTo { get; init; }
}
