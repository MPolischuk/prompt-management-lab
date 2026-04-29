using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services;
using PromptLab.Entities.Prompts;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class PromptsController(IPromptService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PromptSearchRequest request, CancellationToken cancellationToken)
    {
        var result = await service.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] UpsertPromptRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        if (!result.Success)
        {
            return this.ToHttpResult(result);
        }

        if (!result.EntityId.HasValue)
        {
            return BadRequest(result);
        }

        return Created($"/api/prompts/{result.EntityId.Value}", result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpsertPromptRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);
        return this.ToHttpResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return this.ToHttpResult(result);
    }

    [HttpPut("{id:guid}/tags")]
    public async Task<IActionResult> SetTagsAsync(Guid id, [FromBody] IReadOnlyCollection<Guid> tagIds, CancellationToken cancellationToken)
    {
        var result = await service.SetTagsAsync(id, tagIds, cancellationToken);
        return this.ToHttpResult(result);
    }
}
