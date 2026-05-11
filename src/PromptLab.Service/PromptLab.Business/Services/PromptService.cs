using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PromptLab.Business.Configuration;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Services;

/// <summary>
/// Implementa los casos de uso de gestion de prompts y coordinacion de cache.
/// </summary>
public class PromptService(
    IPromptRepository repository,
    IPromptVersionRepository versionRepository,
    IMemoryCache cache,
    IOptions<CacheOptions> cacheOptions) : IPromptService
{
    private const string SearchVersionKey = "prompt:search:version";
    /// <inheritdoc />
    public async Task<OperationResult> CreateAsync(UpsertPromptRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.CreateAsync(request, cancellationToken);
        if (!result.Success)
        {
            return result;
        }

        InvalidatePromptSearchCache();

        if (result.Success && result.EntityId.HasValue && request.TagIds.Count > 0)
        {
            await repository.SetTagsAsync(result.EntityId.Value, request.TagIds, cancellationToken);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<OperationResult> UpdateAsync(Guid id, UpsertPromptRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.UpdateAsync(id, request, cancellationToken);
        if (!result.Success)
        {
            return result;
        }

        InvalidatePromptSearchCache();
        await repository.SetTagsAsync(id, request.TagIds, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await repository.DeleteAsync(id, cancellationToken);
        if (!result.Success)
        {
            return result;
        }

        InvalidatePromptSearchCache();
        return result;
    }

    /// <inheritdoc />
    public Task<Prompt?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return repository.GetByIdAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OperationResult> SetTagsAsync(Guid promptId, IReadOnlyCollection<Guid> tagIds, CancellationToken cancellationToken)
    {
        var result = await repository.SetTagsAsync(promptId, tagIds, cancellationToken);
        if (result.Success)
        {
            InvalidatePromptSearchCache();
        }

        return result;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public Task<IReadOnlyList<PromptVersion>> GetVersionsAsync(Guid promptId, CancellationToken cancellationToken)
    {
        return versionRepository.GetByPromptIdAsync(promptId, cancellationToken);
    }
}
