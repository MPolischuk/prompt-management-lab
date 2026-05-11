using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.TestCases;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class TestCasesController(ITestCaseService cases) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBySuiteAsync([FromQuery] Guid suiteId, CancellationToken cancellationToken)
    {
        var list = await cases.GetBySuiteIdAsync(suiteId, cancellationToken);
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTestCaseRequest request, CancellationToken cancellationToken)
    {
        var result = await cases.CreateAsync(request, cancellationToken);
        if (!result.Success)
        {
            return this.ToHttpResult(result);
        }

        if (!result.EntityId.HasValue)
        {
            return BadRequest(result);
        }

        return Created($"/api/testcases/{result.EntityId.Value}", result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateTestCaseRequest request, CancellationToken cancellationToken)
    {
        var result = await cases.UpdateAsync(id, request, cancellationToken);
        return this.ToHttpResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await cases.DeleteAsync(id, cancellationToken);
        return this.ToHttpResult(result);
    }
}
