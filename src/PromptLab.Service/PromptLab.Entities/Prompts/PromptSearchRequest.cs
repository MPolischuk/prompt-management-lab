using PromptLab.Entities.Common;

namespace PromptLab.Entities.Prompts;

public class PromptSearchRequest : PagedRequest
{
    public string? Query { get; init; }
    public string? Category { get; init; }
    public string? Language { get; init; }
    public bool? IsActive { get; init; }
    public Guid? TagId { get; init; }
    public DateTime? CreatedFrom { get; init; }
    public DateTime? CreatedTo { get; init; }
}
