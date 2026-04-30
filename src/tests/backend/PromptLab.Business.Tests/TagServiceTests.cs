using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using PromptLab.Business.Configuration;
using PromptLab.Business.Services;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.Tags;

namespace PromptLab.Business.Tests;

public class TagServiceTests
{
    [Fact]
    public async Task GetAllAsync_UsesCache_AfterFirstCall()
    {
        var repository = new Mock<ITagRepository>();
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Tag { Id = Guid.NewGuid(), Name = "a", Slug = "a" }]);

        var service = new TagService(
            repository.Object,
            new MemoryCache(new MemoryCacheOptions()),
            Options.Create(new CacheOptions { TagsTtlSeconds = 60 }));

        _ = await service.GetAllAsync(CancellationToken.None);
        _ = await service.GetAllAsync(CancellationToken.None);

        repository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WhenQueryIsEmpty_DelegatesToGetAll()
    {
        var repository = new Mock<ITagRepository>();
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Tag { Id = Guid.NewGuid(), Name = "a", Slug = "a" }]);

        var service = new TagService(
            repository.Object,
            new MemoryCache(new MemoryCacheOptions()),
            Options.Create(new CacheOptions()));

        var result = await service.SearchAsync("", CancellationToken.None);

        result.Should().HaveCount(1);
        repository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenFails_DoesNotInvalidateCache()
    {
        var repository = new Mock<ITagRepository>();
        repository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Tag { Id = Guid.NewGuid(), Name = "cached", Slug = "cached" }]);
        repository.Setup(r => r.CreateAsync(It.IsAny<CreateTagRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = false, ErrorCode = OperationErrorCode.Validation });

        var service = new TagService(
            repository.Object,
            new MemoryCache(new MemoryCacheOptions()),
            Options.Create(new CacheOptions()));

        _ = await service.GetAllAsync(CancellationToken.None);
        _ = await service.CreateAsync(new CreateTagRequest { Name = "new" }, CancellationToken.None);
        _ = await service.GetAllAsync(CancellationToken.None);

        repository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
