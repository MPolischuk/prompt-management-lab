using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services.Contracts;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/ai-providers")]
public class AiProvidersController(IAnalyzeService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var result = await service.GetProvidersAsync(cancellationToken);
        return Ok(result);
    }
}
