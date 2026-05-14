using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Analyze;

namespace PromptLab.Service.Tests;

public class AiProvidersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AiProvidersControllerTests(WebApplicationFactory<Program> factory)
    {
        var analyze = new Mock<IAnalyzeService>();
        analyze.Setup(a => a.GetProvidersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (IReadOnlyCollection<AnalyzeProvider>)
                [
                    new AnalyzeProvider { Name = "openai", Enabled = true, Models = [] }
                ]);

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
    public async Task Get_Returns200WithProviders()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/ai-providers");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
