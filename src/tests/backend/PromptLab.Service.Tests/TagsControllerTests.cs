using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Common;
using PromptLab.Entities.Tags;

namespace PromptLab.Service.Tests;

public class TagsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TagsControllerTests(WebApplicationFactory<Program> factory)
    {
        var tags = new Mock<ITagService>();
        tags.Setup(t => t.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Tag>());
        tags.Setup(t => t.SearchAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Tag>());
        tags.Setup(t => t.CreateAsync(It.IsAny<CreateTagRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });

        _factory = factory.WithWebHostBuilder(b =>
        {
            b.ConfigureServices(services =>
            {
                services.RemoveAll<ITagService>();
                services.AddSingleton(tags.Object);
            });
        });
    }

    [Fact]
    public async Task GetAll_WithoutQuery_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/tags");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_WithQuery_UsesSearch()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/tags?query=test");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_Returns200WhenSuccess()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/tags", new CreateTagRequest { Name = "n" });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
