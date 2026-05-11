using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.TestResults;
using PromptLab.Entities.TestRuns;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class TestRunsController(ITestRunService runs) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] Guid? suiteId, CancellationToken cancellationToken)
    {
        if (suiteId.HasValue)
        {
            var bySuite = await runs.GetBySuiteIdAsync(suiteId.Value, cancellationToken);
            return Ok(bySuite);
        }

        var all = await runs.GetAllAsync(cancellationToken);
        return Ok(all);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var detail = await runs.GetByIdWithResultsAsync(id, cancellationToken);
        return detail is null ? NotFound() : Ok(detail);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTestRunRequest request, CancellationToken cancellationToken)
    {
        var result = await runs.CreateAsync(request, cancellationToken);
        if (!result.Success)
        {
            return this.ToHttpResult(result);
        }

        if (!result.EntityId.HasValue)
        {
            return BadRequest(result);
        }

        return Created($"/api/testruns/{result.EntityId.Value}", result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateTestRunRequest request, CancellationToken cancellationToken)
    {
        var result = await runs.UpdateAsync(id, request, cancellationToken);
        return this.ToHttpResult(result);
    }

    [HttpPost("{runId:guid}/results")]
    public async Task<IActionResult> CreateResultAsync(Guid runId, [FromBody] TestResultCreateBody body, CancellationToken cancellationToken)
    {
        var request = new CreateTestResultRequest
        {
            RunId = runId,
            CaseId = body.CaseId,
            ActualOutput = body.ActualOutput,
            Passed = body.Passed,
            Score = body.Score,
            LatencyMs = body.LatencyMs,
            Error = body.Error
        };

        var result = await runs.CreateResultAsync(request, cancellationToken);
        if (!result.Success)
        {
            return this.ToHttpResult(result);
        }

        if (!result.EntityId.HasValue)
        {
            return BadRequest(result);
        }

        return Created($"/api/testruns/{runId}", result);
    }
}

/// <summary>Cuerpo para crear resultado (runId viene de la ruta).</summary>
public sealed class TestResultCreateBody
{
    public Guid CaseId { get; init; }
    public required string ActualOutput { get; init; }
    public bool Passed { get; init; }
    public decimal Score { get; init; }
    public int LatencyMs { get; init; }
    public string? Error { get; init; }
}
