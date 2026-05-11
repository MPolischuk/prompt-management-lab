using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services.Contracts;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/Prompts")]
public class PromptVersionsController(IPromptService prompts) : ControllerBase
{
    /// <summary>Lista el historial de versiones de contenido de un prompt.</summary>
    [HttpGet("{promptId:guid}/versions")]
    public async Task<IActionResult> GetVersionsAsync(Guid promptId, CancellationToken cancellationToken)
    {
        var versions = await prompts.GetVersionsAsync(promptId, cancellationToken);
        return Ok(versions);
    }
}
