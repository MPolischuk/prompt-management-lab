using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Prompts;

namespace PromptLab.Service.Tests;

public class PromptVersionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PromptVersionsControllerTests(WebApplicationFactory<Program> factory)
    {
        var prompts = new Mock<IPromptService>();
        prompts.Setup(p => p.GetVersionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                (IReadOnlyList<PromptVersion>)
                [
                    new PromptVersion
                    {
                        Id = Guid.NewGuid(),
                        PromptId = Guid.NewGuid(),
                        Content = "v",
                        Version = 1,
                        CreatedAt = DateTime.UtcNow
                    }
                ]);

        _factory = factory.WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<IPromptService>();
                services.AddSingleton(prompts.Object);
            });
        });
    }

    [Fact]
    public async Task GetVersions_Returns200WithList()
    {
        using var client = _factory.CreateClient();
        var promptId = Guid.NewGuid();
        var response = await client.GetAsync($"/api/prompts/{promptId}/versions");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
