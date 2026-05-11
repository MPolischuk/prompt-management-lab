namespace PromptLab.Entities.Prompts;

public class Prompt
{
    public Guid Id { get; init; }
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
    public int Version { get; init; } = 1;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    /// <summary>Nombres de tags (compatibilidad).</summary>
    public IReadOnlyCollection<string> Tags { get; init; } = [];
    /// <summary>Tags con identificador para el cliente.</summary>
    public IReadOnlyCollection<PromptTagSummary> TagSummaries { get; init; } = [];
}
