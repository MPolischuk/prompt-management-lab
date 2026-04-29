using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services;
using PromptLab.Entities.Analyze;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
/// <summary>
/// Expone endpoints para ejecutar analisis y consultar resultados.
/// </summary>
public class AnalyzeController(IAnalyzeService service) : ControllerBase
{
    /// <summary>Ejecuta un analisis para el request recibido.</summary>
    [HttpPost]
    public async Task<IActionResult> AnalyzeAsync([FromBody] AnalyzeRequest request, CancellationToken cancellationToken)
    {
        var result = await service.AnalyzeAsync(request, cancellationToken);
        return this.ToHttpResult(result);
    }

    /// <summary>Obtiene una corrida de analisis por identificador.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }
}
