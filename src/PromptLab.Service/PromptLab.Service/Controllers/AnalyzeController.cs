using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services;
using PromptLab.Entities.Analyze;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class AnalyzeController(IAnalyzeService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AnalyzeAsync([FromBody] AnalyzeRequest request, CancellationToken cancellationToken)
    {
        var result = await service.AnalyzeAsync(request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
