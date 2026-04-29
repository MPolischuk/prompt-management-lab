namespace PromptLab.Entities.Analyze;

public class AiModel
{
    public required string Id { get; init; }
    public required string Provider { get; init; }
    public required string DisplayName { get; init; }
    public bool Enabled { get; init; } = true;
}
