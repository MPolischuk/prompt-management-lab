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
    public decimal? Temperature { get; init; }
    public int? MaxTokens { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public IReadOnlyCollection<string> Tags { get; init; } = [];
}
