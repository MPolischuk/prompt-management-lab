using Microsoft.AspNetCore.Mvc;
using PromptLab.Entities.Common;

namespace PromptLab.Service.Controllers;

/// <summary>
/// Convierte un <see cref="OperationResult"/> en una respuesta HTTP consistente.
/// </summary>
internal static class OperationResultHttpMapper
{
    /// <summary>Mapea el resultado de negocio a un codigo HTTP.</summary>
    public static IActionResult ToHttpResult(this ControllerBase controller, OperationResult result)
    {
        if (result.Success)
        {
            return controller.Ok(result);
        }

        return result.ErrorCode switch
        {
            OperationErrorCode.NotFound => controller.NotFound(result),
            OperationErrorCode.Conflict => controller.Conflict(result),
            OperationErrorCode.Unavailable => controller.StatusCode(StatusCodes.Status503ServiceUnavailable, result),
            OperationErrorCode.Validation => controller.BadRequest(result),
            _ => controller.BadRequest(result)
        };
    }
}
