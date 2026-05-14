using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Analyze;

namespace PromptLab.Service.Tests;

public class AiModelsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AiModelsControllerTests(WebApplicationFactory<Program> factory)
    {
        var analyze = new Mock<IAnalyzeService>();
        analyze.Setup(a => a.GetModelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (IReadOnlyCollection<AiModel>)
                [
                    new AiModel { Id = "m1", Provider = "openai", DisplayName = "M1" }
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
    public async Task Get_Returns200WithModels()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/ai-models");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
