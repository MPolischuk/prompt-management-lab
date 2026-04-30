using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PromptLab.Business.Configuration;
using PromptLab.Business.Services.Contracts;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.Tags;

namespace PromptLab.Business.Services;

/// <summary>
/// Implementa los casos de uso de gestion de tags y cache asociado.
/// </summary>
public class TagService(
    ITagRepository repository,
    IMemoryCache cache,
    IOptions<CacheOptions> cacheOptions) : ITagService
{
    private const string TagsAllCacheKey = "tags:all";

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<Tag>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(TagsAllCacheKey, out IReadOnlyCollection<Tag>? cached) && cached is not null)
        {
            return cached;
        }

        var result = await repository.GetAllAsync(cancellationToken);
        cache.Set(TagsAllCacheKey, result, TimeSpan.FromSeconds(cacheOptions.Value.TagsTtlSeconds));
        return result;
    }

    /// <inheritdoc />
    public Task<IReadOnlyCollection<Tag>> SearchAsync(string? query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllAsync(cancellationToken);
        }

        return repository.SearchAsync(query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<OperationResult> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.CreateAsync(request, cancellationToken);
        if (result.Success)
        {
            cache.Remove(TagsAllCacheKey);
        }

        return result;
    }
}
