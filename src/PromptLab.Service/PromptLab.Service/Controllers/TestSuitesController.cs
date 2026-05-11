using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.TestSuites;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class TestSuitesController(ITestSuiteService suites) : ControllerBase
{
    /// <summary>Suites de un prompt (query promptId).</summary>
    [HttpGet]
    public async Task<IActionResult> GetByPromptAsync([FromQuery] Guid promptId, CancellationToken cancellationToken)
    {
        var list = await suites.GetByPromptIdAsync(promptId, cancellationToken);
        return Ok(list);
    }

    /// <summary>Detalle de suite con casos de prueba.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var detail = await suites.GetByIdWithCasesAsync(id, cancellationToken);
        return detail is null ? NotFound() : Ok(detail);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTestSuiteRequest request, CancellationToken cancellationToken)
    {
        var result = await suites.CreateAsync(request, cancellationToken);
        if (!result.Success)
        {
            return this.ToHttpResult(result);
        }

        if (!result.EntityId.HasValue)
        {
            return BadRequest(result);
        }

        return Created($"/api/testsuites/{result.EntityId.Value}", result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateTestSuiteRequest request, CancellationToken cancellationToken)
    {
        var result = await suites.UpdateAsync(id, request, cancellationToken);
        return this.ToHttpResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await suites.DeleteAsync(id, cancellationToken);
        return this.ToHttpResult(result);
    }
}
