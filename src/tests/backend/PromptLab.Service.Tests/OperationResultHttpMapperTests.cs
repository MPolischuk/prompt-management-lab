using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromptLab.Entities.Common;
using PromptLab.Service.Controllers;

namespace PromptLab.Service.Tests;

public class OperationResultHttpMapperTests
{
    private sealed class StubController : ControllerBase;

    private static StubController CreateController()
    {
        var c = new StubController
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
        return c;
    }

    [Fact]
    public void ToHttpResult_WhenSuccess_Returns200WithBody()
    {
        var c = CreateController();
        var payload = new OperationResult { Success = true, Message = "done" };

        var action = c.ToHttpResult(payload);

        var ok = action.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(payload);
    }

    [Fact]
    public void ToHttpResult_WhenNotFound_Returns404()
    {
        var c = CreateController();
        var payload = new OperationResult { Success = false, ErrorCode = OperationErrorCode.NotFound, Message = "nf" };

        var action = c.ToHttpResult(payload);

        var nf = action.Should().BeOfType<NotFoundObjectResult>().Subject;
        nf.Value.Should().BeSameAs(payload);
    }

    [Fact]
    public void ToHttpResult_WhenConflict_Returns409()
    {
        var c = CreateController();
        var payload = new OperationResult { Success = false, ErrorCode = OperationErrorCode.Conflict };

        var action = c.ToHttpResult(payload);

        action.Should().BeOfType<ConflictObjectResult>().Which.Value.Should().BeSameAs(payload);
    }

    [Fact]
    public void ToHttpResult_WhenUnavailable_Returns503()
    {
        var c = CreateController();
        var payload = new OperationResult { Success = false, ErrorCode = OperationErrorCode.Unavailable };

        var action = c.ToHttpResult(payload);

        var obj = action.Should().BeOfType<ObjectResult>().Subject;
        obj.StatusCode.Should().Be(StatusCodes.Status503ServiceUnavailable);
        obj.Value.Should().BeSameAs(payload);
    }

    [Fact]
    public void ToHttpResult_WhenValidation_Returns400WithMessage()
    {
        var c = CreateController();
        var payload = new OperationResult { Success = false, ErrorCode = OperationErrorCode.Validation, Message = "bad" };

        var action = c.ToHttpResult(payload);

        var br = action.Should().BeOfType<BadRequestObjectResult>().Subject;
        br.Value.Should().BeSameAs(payload);
    }

    [Fact]
    public void ToHttpResult_WhenUnexpected_Returns400()
    {
        var c = CreateController();
        var payload = new OperationResult { Success = false, ErrorCode = OperationErrorCode.Unexpected };

        var action = c.ToHttpResult(payload);

        action.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().BeSameAs(payload);
    }
}
