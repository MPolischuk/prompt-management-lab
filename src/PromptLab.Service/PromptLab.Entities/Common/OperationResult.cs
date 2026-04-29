namespace PromptLab.Entities.Common;

public class OperationResult
{
    public bool Success { get; init; }
    public Guid? EntityId { get; init; }
    public string? Message { get; init; }
    public OperationErrorCode ErrorCode { get; init; } = OperationErrorCode.None;
}

public enum OperationErrorCode
{
    None = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unavailable = 4,
    Unexpected = 5
}
