using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PromptLab.Business.Configuration;
using PromptLab.Data.Repositories;
using PromptLab.Entities.Common;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Services;

public class PromptService(
    IPromptRepository repository,
    IMemoryCache cache,
    IOptions<CacheOptions> cacheOptions) : IPromptService
{
    private const string SearchVersionKey = "prompt:search:version";
    public async Task<OperationResult> CreateAsync(UpsertPromptRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.CreateAsync(request, cancellationToken);
        InvalidatePromptSearchCache();

        if (result.Success && result.EntityId.HasValue && request.TagIds.Count > 0)
        {
            await repository.SetTagsAsync(result.EntityId.Value, request.TagIds, cancellationToken);
        }

        return result;
    }

    public async Task<OperationResult> UpdateAsync(Guid id, UpsertPromptRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.UpdateAsync(id, request, cancellationToken);
        InvalidatePromptSearchCache();

        if (result.Success)
        {
            await repository.SetTagsAsync(id, request.TagIds, cancellationToken);
        }

        return result;
    }

    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await repository.DeleteAsync(id, cancellationToken);
        InvalidatePromptSearchCache();
        return result;
    }

    public Task<Prompt?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return repository.GetByIdAsync(id, cancellationToken);
    }

    public Task<OperationResult> SetTagsAsync(Guid promptId, IReadOnlyCollection<Guid> tagIds, CancellationToken cancellationToken)
    {
        InvalidatePromptSearchCache();
        return repository.SetTagsAsync(promptId, tagIds, cancellationToken);
    }

    public async Task<PagedResponse<Prompt>> SearchAsync(PromptSearchRequest request, CancellationToken cancellationToken)
    {
        var cacheKey = BuildSearchKey(request);
        if (cache.TryGetValue(cacheKey, out PagedResponse<Prompt>? cached) && cached is not null)
        {
            return cached;
        }

        var result = await repository.SearchAsync(request, cancellationToken);
        cache.Set(cacheKey, result, TimeSpan.FromSeconds(cacheOptions.Value.PromptSearchTtlSeconds));
        return result;
    }

    private void InvalidatePromptSearchCache()
    {
        var currentVersion = cache.Get<int>(SearchVersionKey);
        cache.Set(SearchVersionKey, currentVersion + 1);
    }

    private string BuildSearchKey(PromptSearchRequest request)
    {
        var version = cache.Get<int>(SearchVersionKey);
        return $"prompt:search:v{version}:{request.Query}:{request.Category}:{request.Language}:{request.IsActive}:{request.TagId}:{request.CreatedFrom}:{request.CreatedTo}:{request.PageNumber}:{request.PageSize}";
    }
}
