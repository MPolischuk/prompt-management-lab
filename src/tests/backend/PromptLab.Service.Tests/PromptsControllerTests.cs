using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Common;
using PromptLab.Entities.Prompts;

namespace PromptLab.Service.Tests;

public class PromptsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PromptsControllerTests(WebApplicationFactory<Program> factory)
    {
        var prompts = new Mock<IPromptService>();
        prompts.Setup(p => p.SearchAsync(It.IsAny<PromptSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResponse<Prompt> { Items = [], PageNumber = 1, PageSize = 20, TotalRows = 0 });
        prompts.Setup(p => p.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Prompt?)null);
        prompts.Setup(p => p.CreateAsync(It.IsAny<UpsertPromptRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });
        prompts.Setup(p => p.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpsertPromptRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        prompts.Setup(p => p.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        prompts.Setup(p => p.SetTagsAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });

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
    public async Task Search_GetsPagedResult()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/prompts?pageNumber=1&pageSize=10");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_WhenMissing_Returns404()
    {
        using var client = _factory.CreateClient();
        var response = await client.GetAsync($"/api/prompts/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_WhenSuccessful_Returns201()
    {
        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/prompts",
            new UpsertPromptRequest { Title = "t", Content = "c" });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Update_WhenSuccessful_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.PutAsJsonAsync(
            $"/api/prompts/{Guid.NewGuid()}",
            new UpsertPromptRequest { Title = "t", Content = "c" });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_WhenSuccessful_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.DeleteAsync($"/api/prompts/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SetTags_WhenSuccessful_Returns200()
    {
        using var client = _factory.CreateClient();
        var response = await client.PutAsJsonAsync(
            $"/api/prompts/{Guid.NewGuid()}/tags",
            new[] { Guid.NewGuid() });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
