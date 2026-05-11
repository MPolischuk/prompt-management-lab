namespace PromptLab.Entities.Prompts;

public sealed class PromptVersion
{
    public Guid Id { get; init; }
    public Guid PromptId { get; init; }
    public required string Content { get; init; }
    public int Version { get; init; }
    public DateTime CreatedAt { get; init; }
}
