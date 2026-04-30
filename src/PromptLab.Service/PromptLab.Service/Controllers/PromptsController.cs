using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Prompts;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
/// <summary>
/// Expone endpoints CRUD y busqueda para prompts.
/// </summary>
public class PromptsController(IPromptService service) : ControllerBase
{
    /// <summary>Busca prompts aplicando filtros opcionales.</summary>
    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PromptSearchRequest request, CancellationToken cancellationToken)
    {
        var result = await service.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>Obtiene un prompt por identificador.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Crea un nuevo prompt.</summary>
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

    /// <summary>Actualiza un prompt existente.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpsertPromptRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);
        return this.ToHttpResult(result);
    }

    /// <summary>Elimina un prompt por identificador.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return this.ToHttpResult(result);
    }

    /// <summary>Asocia una lista de tags a un prompt.</summary>
    [HttpPut("{id:guid}/tags")]
    public async Task<IActionResult> SetTagsAsync(Guid id, [FromBody] IReadOnlyCollection<Guid> tagIds, CancellationToken cancellationToken)
    {
        var result = await service.SetTagsAsync(id, tagIds, cancellationToken);
        return this.ToHttpResult(result);
    }
}
