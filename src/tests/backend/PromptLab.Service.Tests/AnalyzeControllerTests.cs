using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;

namespace PromptLab.Service.Tests;

public class AnalyzeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AnalyzeControllerTests(WebApplicationFactory<Program> factory)
    {
        var analyze = new Mock<IAnalyzeService>();
        analyze.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AnalyzeRun?)null);
        analyze.Setup(s => s.AnalyzeAsync(It.IsAny<AnalyzeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });

        _factory = factory.WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<IAnalyzeService>();
                services.AddSingleton(analyze.Object);
            });
        });
    }

    [Fact]
    public async Task GetByIdAsync_WhenRunDoesNotExist_Returns404()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/analyze/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRunExists_Returns200WithPayload()
    {
        var runId = Guid.NewGuid();
        var analyze = new Mock<IAnalyzeService>();
        analyze.Setup(s => s.GetByIdAsync(runId, It.IsAny<CancellationToken>())).ReturnsAsync(
            new AnalyzeRun
            {
                Id = runId,
                PromptId = Guid.NewGuid(),
                Provider = "simulated",
                ModelId = "simulated-default",
                Status = "Completed",
                Output = "ok",
                CreatedAt = DateTime.UtcNow
            });
        analyze.Setup(s => s.GetByIdAsync(It.Is<Guid>(g => g != runId), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AnalyzeRun?)null);
        analyze.Setup(s => s.AnalyzeAsync(It.IsAny<AnalyzeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<IAnalyzeService>();
                services.AddSingleton(analyze.Object);
            });
        });

        using var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/analyze/{runId}");
        var payload = await response.Content.ReadFromJsonAsync<AnalyzeRun>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        payload.Should().NotBeNull();
        payload!.Id.Should().Be(runId);
        payload.Output.Should().Be("ok");
    }

    [Fact]
    public async Task Post_WhenAnalyzeSucceeds_Returns200Ok()
    {
        var analyze = new Mock<IAnalyzeService>();
        analyze.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AnalyzeRun?)null);
        analyze.Setup(s => s.AnalyzeAsync(It.IsAny<AnalyzeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid(), Message = "ok" });

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<IAnalyzeService>();
                services.AddSingleton(analyze.Object);
            });
        });

        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/analyze",
            new AnalyzeRequest { PromptId = Guid.NewGuid(), Input = "x" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        analyze.Verify(s => s.AnalyzeAsync(It.IsAny<AnalyzeRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Post_WhenServiceReturnsFailure_ReturnsExpectedHttpStatus()
    {
        var analyze = new Mock<IAnalyzeService>();
        analyze.Setup(s => s.AnalyzeAsync(It.IsAny<AnalyzeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = false, ErrorCode = OperationErrorCode.NotFound, Message = "missing" });

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<IAnalyzeService>();
                services.AddSingleton(analyze.Object);
            });
        });

        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/analyze",
            new AnalyzeRequest { PromptId = Guid.NewGuid(), Input = "x" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
