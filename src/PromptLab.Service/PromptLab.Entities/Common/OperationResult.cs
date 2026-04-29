namespace PromptLab.Entities.Common;

public class OperationResult
{
    public bool Success { get; init; }
    public Guid? EntityId { get; init; }
    public string? Message { get; init; }
}
