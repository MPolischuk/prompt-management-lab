namespace PromptLab.Entities.Prompts;

/// <summary>Tag asociado a un prompt (para UI y PUT de tags).</summary>
public sealed class PromptTagSummary
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
}
