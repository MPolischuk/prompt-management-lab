using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services;
using PromptLab.Entities.Tags;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
/// <summary>
/// Expone endpoints para consulta y creacion de tags.
/// </summary>
public class TagsController(ITagService service) : ControllerBase
{
    /// <summary>Obtiene todos los tags o filtra por query.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] string? query, CancellationToken cancellationToken)
    {
        var result = string.IsNullOrWhiteSpace(query)
            ? await service.GetAllAsync(cancellationToken)
            : await service.SearchAsync(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>Crea un nuevo tag.</summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTagRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return this.ToHttpResult(result);
    }
}
