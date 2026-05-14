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

namespace PromptLab.Service.Tests;

public class TestCasesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TestCasesControllerTests(WebApplicationFactory<Program> factory)
    {
        var cases = new Mock<ITestCaseService>();
        cases.Setup(c => c.GetBySuiteIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TestCase>());
        cases.Setup(c => c.CreateAsync(It.IsAny<CreateTestCaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });
        cases.Setup(c => c.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateTestCaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        cases.Setup(c => c.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });

        _factory = factory.WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<ITestCaseService>();
                services.AddSingleton(cases.Object);
            });
        });
    }

    [Fact]
    public async Task GetBySuite_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/testcases?suiteId={Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WhenSuccessful_Returns201()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/testcases",
            new CreateTestCaseRequest
            {
                SuiteId = Guid.NewGuid(),
                Name = "c",
                InputVariables = "{}",
                ExpectedOutput = null
            });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Update_WhenSuccessful_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.PutAsJsonAsync(
            $"/api/testcases/{Guid.NewGuid()}",
            new UpdateTestCaseRequest { Name = "c", InputVariables = "{}", ExpectedOutput = null });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_WhenSuccessful_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.DeleteAsync($"/api/testcases/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_WhenSuccessButNoEntityId_Returns400()
    {
        var cases = new Mock<ITestCaseService>();
        WireCaseDefaults(cases);
        cases.Setup(c => c.CreateAsync(It.IsAny<CreateTestCaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = null });

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<ITestCaseService>();
                services.AddSingleton(cases.Object);
            });
        });

        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/testcases",
            new CreateTestCaseRequest
            {
                SuiteId = Guid.NewGuid(),
                Name = "c",
                InputVariables = "{}",
                ExpectedOutput = null
            });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_WhenNotFound_Returns404()
    {
        var cases = new Mock<ITestCaseService>();
        WireCaseDefaults(cases);
        cases.Setup(c => c.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateTestCaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = false, ErrorCode = OperationErrorCode.NotFound });

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<ITestCaseService>();
                services.AddSingleton(cases.Object);
            });
        });

        using var client = factory.CreateClient();
        var response = await client.PutAsJsonAsync(
            $"/api/testcases/{Guid.NewGuid()}",
            new UpdateTestCaseRequest { Name = "c", InputVariables = "{}", ExpectedOutput = null });
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WhenConflict_Returns409()
    {
        var cases = new Mock<ITestCaseService>();
        WireCaseDefaults(cases);
        cases.Setup(c => c.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = false, ErrorCode = OperationErrorCode.Conflict });

        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<ITestCaseService>();
                services.AddSingleton(cases.Object);
            });
        });

        using var client = factory.CreateClient();
        var response = await client.DeleteAsync($"/api/testcases/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    private static void WireCaseDefaults(Mock<ITestCaseService> cases)
    {
        cases.Setup(c => c.GetBySuiteIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<TestCase>());
        cases.Setup(c => c.CreateAsync(It.IsAny<CreateTestCaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });
        cases.Setup(c => c.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateTestCaseRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        cases.Setup(c => c.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
    }
}
