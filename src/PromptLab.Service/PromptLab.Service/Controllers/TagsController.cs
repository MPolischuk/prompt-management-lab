using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Business.Services;
using PromptLab.Entities.Tags;

namespace PromptLab.Service.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class TagsController(ITagService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] string? query, CancellationToken cancellationToken)
    {
        var result = string.IsNullOrWhiteSpace(query)
            ? await service.GetAllAsync(cancellationToken)
            : await service.SearchAsync(query, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTagRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
