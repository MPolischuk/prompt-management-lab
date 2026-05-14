using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Common;
using PromptLab.Entities.TestCases;
using PromptLab.Entities.TestSuites;

namespace PromptLab.Service.Tests;

public class TestSuitesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TestSuitesControllerTests(WebApplicationFactory<Program> factory)
    {
        var suites = new Mock<ITestSuiteService>();
        suites.Setup(s => s.GetByPromptIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestSuite>());
        suites.Setup(s => s.GetByIdWithCasesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestSuiteDetail?)null);
        suites.Setup(s => s.CreateAsync(It.IsAny<CreateTestSuiteRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });
        suites.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateTestSuiteRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        suites.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });

        _factory = factory.WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<ITestSuiteService>();
                services.AddSingleton(suites.Object);
            });
        });
    }

    [Fact]
    public async Task GetByPrompt_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/testsuites?promptId={Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/testsuites/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WhenSuccessful_Returns201()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/testsuites",
            new CreateTestSuiteRequest { PromptId = Guid.NewGuid(), Name = "s", Description = null });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Update_WhenSuccessful_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.PutAsJsonAsync(
            $"/api/testsuites/{Guid.NewGuid()}",
            new UpdateTestSuiteRequest { Name = "s", Description = null });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_WhenSuccessful_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.DeleteAsync($"/api/testsuites/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenFound_Returns200WithPayload()
    {
        var id = Guid.NewGuid();
        var detail = new TestSuiteDetail
        {
            Suite = new TestSuite
            {
                Id = id,
                PromptId = Guid.NewGuid(),
                Name = "S",
                Description = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            Cases = []
        };
        var suites = new Mock<ITestSuiteService>();
        suites.Setup(s => s.GetByIdWithCasesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns((Guid suiteId, CancellationToken _) => Task.FromResult<TestSuiteDetail?>(suiteId == id ? detail : null));
        suites.Setup(s => s.GetByPromptIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<TestSuite>());
        suites.Setup(s => s.CreateAsync(It.IsAny<CreateTestSuiteRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });
        suites.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateTestSuiteRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        suites.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<ITestSuiteService>();
                services.AddSingleton(suites.Object);
            });
        });

        using var client = factory.CreateClient();
        var response = await client.GetAsync($"/api/testsuites/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WhenSuccessButNoEntityId_Returns400()
    {
        var suites = new Mock<ITestSuiteService>();
        WireSuiteDefaults(suites);
        suites.Setup(s => s.CreateAsync(It.IsAny<CreateTestSuiteRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = null });

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<ITestSuiteService>();
                services.AddSingleton(suites.Object);
            });
        });

        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/testsuites",
            new CreateTestSuiteRequest { PromptId = Guid.NewGuid(), Name = "s", Description = null });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_WhenNotFound_Returns404()
    {
        var suites = new Mock<ITestSuiteService>();
        WireSuiteDefaults(suites);
        suites.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateTestSuiteRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = false, ErrorCode = OperationErrorCode.NotFound });

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<ITestSuiteService>();
                services.AddSingleton(suites.Object);
            });
        });

        using var client = factory.CreateClient();
        var response = await client.PutAsJsonAsync(
            $"/api/testsuites/{Guid.NewGuid()}",
            new UpdateTestSuiteRequest { Name = "s", Description = null });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static void WireSuiteDefaults(Mock<ITestSuiteService> suites)
    {
        suites.Setup(s => s.GetByPromptIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<TestSuite>());
        suites.Setup(s => s.GetByIdWithCasesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((TestSuiteDetail?)null);
        suites.Setup(s => s.CreateAsync(It.IsAny<CreateTestSuiteRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });
        suites.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateTestSuiteRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        suites.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
    }
}
