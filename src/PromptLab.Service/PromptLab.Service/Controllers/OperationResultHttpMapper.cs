using Microsoft.AspNetCore.Mvc;
using PromptLab.Entities.Common;

namespace PromptLab.Service.Controllers;

internal static class OperationResultHttpMapper
{
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
