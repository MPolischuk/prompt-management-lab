namespace PromptLab.Entities.Common;

/// <summary>
/// Representa el resultado estandar de una operacion de negocio.
/// </summary>
public class OperationResult
{
    /// <summary>Indica si la operacion fue exitosa.</summary>
    public bool Success { get; init; }
    /// <summary>Identificador de entidad afectada, cuando aplica.</summary>
    public Guid? EntityId { get; init; }
    /// <summary>Mensaje descriptivo para consumidor y UI.</summary>
    public string? Message { get; init; }
    /// <summary>Codigo de error de negocio para mapeo HTTP.</summary>
    public OperationErrorCode ErrorCode { get; init; } = OperationErrorCode.None;
}

/// <summary>
/// Clasifica fallos de negocio en categorias estandar.
/// </summary>
public enum OperationErrorCode
{
    None = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unavailable = 4,
    Unexpected = 5
}
