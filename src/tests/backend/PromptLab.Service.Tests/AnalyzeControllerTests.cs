using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.Prompts;

namespace PromptLab.Service.Tests;

public class AnalyzeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AnalyzeControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IPromptRepository>();
                services.RemoveAll<IAnalyzeRepository>();
                services.AddSingleton<IPromptRepository, InMemoryPromptRepository>();
                services.AddSingleton<IAnalyzeRepository, InMemoryAnalyzeRepository>();
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
        var repository = _factory.Services.GetRequiredService<IAnalyzeRepository>() as InMemoryAnalyzeRepository;
        repository.Should().NotBeNull();
        repository!.Seed(
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

        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/analyze/{runId}");
        var payload = await response.Content.ReadFromJsonAsync<AnalyzeRun>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        payload.Should().NotBeNull();
        payload!.Id.Should().Be(runId);
        payload.Output.Should().Be("ok");
    }

    private sealed class InMemoryPromptRepository : IPromptRepository
    {
        public Task<OperationResult> CreateAsync(UpsertPromptRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new OperationResult { Success = true, EntityId = Guid.NewGuid() });

        public Task<OperationResult> UpdateAsync(Guid id, UpsertPromptRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new OperationResult { Success = true, EntityId = id });

        public Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult(new OperationResult { Success = true, EntityId = id });

        public Task<Prompt?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult<Prompt?>(null);

        public Task<PagedResponse<Prompt>> SearchAsync(PromptSearchRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(new PagedResponse<Prompt> { Items = [], PageNumber = 1, PageSize = 10, TotalRows = 0 });

        public Task<OperationResult> SetTagsAsync(Guid promptId, IReadOnlyCollection<Guid> tagIds, CancellationToken cancellationToken) =>
            Task.FromResult(new OperationResult { Success = true, EntityId = promptId });
    }

    private sealed class InMemoryAnalyzeRepository : IAnalyzeRepository
    {
        private readonly Dictionary<Guid, AnalyzeRun> _runs = new();

        public void Seed(AnalyzeRun run) => _runs[run.Id] = run;

        public Task<OperationResult> CreateRunAsync(AnalyzeRun run, CancellationToken cancellationToken)
        {
            _runs[run.Id] = run;
            return Task.FromResult(new OperationResult { Success = true, EntityId = run.Id });
        }

        public Task<AnalyzeRun?> GetRunByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _runs.TryGetValue(id, out var run);
            return Task.FromResult(run);
        }
    }
}
