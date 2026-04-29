using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PromptLab.Business.Configuration;
using PromptLab.Data.Repositories;
using PromptLab.Entities.Common;
using PromptLab.Entities.Tags;

namespace PromptLab.Business.Services;

public class TagService(
    ITagRepository repository,
    IMemoryCache cache,
    IOptions<CacheOptions> cacheOptions) : ITagService
{
    private const string TagsAllCacheKey = "tags:all";

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

    public Task<IReadOnlyCollection<Tag>> SearchAsync(string? query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllAsync(cancellationToken);
        }

        return repository.SearchAsync(query, cancellationToken);
    }

    public async Task<OperationResult> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.CreateAsync(request, cancellationToken);
        cache.Remove(TagsAllCacheKey);
        return result;
    }
}
