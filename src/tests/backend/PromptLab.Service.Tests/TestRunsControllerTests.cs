using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Common;
using PromptLab.Entities.TestResults;
using PromptLab.Entities.TestRuns;
using PromptLab.Service.Controllers;

namespace PromptLab.Service.Tests;

public class TestRunsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TestRunsControllerTests(WebApplicationFactory<Program> factory)
    {
        var runs = new Mock<ITestRunService>();
        runs.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestRun>());
        runs.Setup(r => r.GetBySuiteIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestRun>());
        runs.Setup(r => r.GetByIdWithResultsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestRunDetail?)null);
        runs.Setup(r => r.CreateAsync(It.IsAny<CreateTestRunRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });
        runs.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateTestRunRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        runs.Setup(r => r.CreateResultAsync(It.IsAny<CreateTestResultRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });

        _factory = factory.WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<ITestRunService>();
                services.AddSingleton(runs.Object);
            });
        });
    }

    [Fact]
    public async Task GetAll_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/testruns");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBySuite_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/testruns?suiteId={Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/testruns/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WhenSuccessful_Returns201()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/testruns",
            new CreateTestRunRequest
            {
                SuiteId = Guid.NewGuid(),
                PromptId = Guid.NewGuid(),
                PromptVersion = 1,
                Model = "m",
                Temperature = 0.1m,
                Status = "Pending"
            });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Update_WhenSuccessful_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.PutAsJsonAsync(
            $"/api/testruns/{Guid.NewGuid()}",
            new UpdateTestRunRequest { Status = "Done", StartedAt = DateTime.UtcNow, CompletedAt = DateTime.UtcNow });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateResult_WhenSuccessful_Returns201()
    {
        using var client = _factory.CreateClient();
        var runId = Guid.NewGuid();
        var response = await client.PostAsJsonAsync(
            $"/api/testruns/{runId}/results",
            new TestResultCreateBody
            {
                CaseId = Guid.NewGuid(),
                ActualOutput = "out",
                Passed = true,
                Score = 1m,
                LatencyMs = 5,
                Error = null
            });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
