namespace PromptLab.Entities.Prompts;

public class UpsertPromptRequest
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string Content { get; init; }
    public string? Category { get; init; }
    public string? Language { get; init; }
    public string? ModelHint { get; init; }
    public string? TargetModelId { get; init; }
    public decimal? Temperature { get; init; }
    public int? MaxTokens { get; init; }
    public decimal? TopP { get; init; }
    public bool IsActive { get; init; } = true;
    public IReadOnlyCollection<Guid> TagIds { get; init; } = [];
}
