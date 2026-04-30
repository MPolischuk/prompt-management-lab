using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services.Contracts;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/ai-models")]
public class AiModelsController(IAnalyzeService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var result = await service.GetModelsAsync(cancellationToken);
        return Ok(result);
    }
}
