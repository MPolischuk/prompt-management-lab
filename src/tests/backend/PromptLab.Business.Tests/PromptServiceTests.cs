using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using PromptLab.Business.Configuration;
using PromptLab.Business.Services;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Tests;

public class PromptServiceTests
{
    private static Mock<IPromptVersionRepository> CreateVersionRepositoryMock()
    {
        var mock = new Mock<IPromptVersionRepository>();
        mock.Setup(v => v.GetByPromptIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<PromptVersion>());
        return mock;
    }

    [Fact]
    public async Task CreateAsync_WhenSuccessfulWithTags_SetsTagsAndInvalidatesCache()
    {
        var createdId = Guid.NewGuid();
        var tagIds = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var repository = new Mock<IPromptRepository>();
        repository.Setup(r => r.CreateAsync(It.IsAny<UpsertPromptRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = createdId });
        repository.Setup(r => r.SetTagsAsync(createdId, It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });

        var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set("prompt:search:version", 2);
        var service = new PromptService(repository.Object, CreateVersionRepositoryMock().Object, cache, Options.Create(new CacheOptions()));

        var result = await service.CreateAsync(
            new UpsertPromptRequest { Title = "t", Content = "c", TagIds = tagIds },
            CancellationToken.None);

        result.Success.Should().BeTrue();
        cache.Get<int>("prompt:search:version").Should().Be(3);
        repository.Verify(r => r.SetTagsAsync(createdId, tagIds, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryFails_DoesNotInvalidateCache()
    {
        var repository = new Mock<IPromptRepository>();
        repository.Setup(r => r.CreateAsync(It.IsAny<UpsertPromptRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = false, ErrorCode = OperationErrorCode.Validation });

        var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set("prompt:search:version", 5);
        var service = new PromptService(repository.Object, CreateVersionRepositoryMock().Object, cache, Options.Create(new CacheOptions()));

        var result = await service.CreateAsync(new UpsertPromptRequest { Title = "t", Content = "c" }, CancellationToken.None);

        result.Success.Should().BeFalse();
        cache.Get<int>("prompt:search:version").Should().Be(5);
    }

    [Fact]
    public async Task DeleteAsync_WhenSuccessful_InvalidatesCache()
    {
        var repository = new Mock<IPromptRepository>();
        repository.Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set("prompt:search:version", 4);
        var sut = new PromptService(repository.Object, CreateVersionRepositoryMock().Object, cache, Options.Create(new CacheOptions()));

        var result = await sut.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        result.Success.Should().BeTrue();
        cache.Get<int>("prompt:search:version").Should().Be(5);
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryFails_DoesNotInvalidateCache()
    {
        var repository = new Mock<IPromptRepository>();
        repository.Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = false, ErrorCode = OperationErrorCode.NotFound });
        var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set("prompt:search:version", 9);
        var sut = new PromptService(repository.Object, CreateVersionRepositoryMock().Object, cache, Options.Create(new CacheOptions()));

        var result = await sut.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        result.Success.Should().BeFalse();
        cache.Get<int>("prompt:search:version").Should().Be(9);
    }

    [Fact]
    public async Task GetByIdAsync_DelegatesToRepository()
    {
        var id = Guid.NewGuid();
        var prompt = new Prompt
        {
            Id = id,
            Title = "t",
            Content = "c",
            Version = 1,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Tags = [],
            TagSummaries = []
        };
        var repository = new Mock<IPromptRepository>();
        repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(prompt);
        var sut = new PromptService(repository.Object, CreateVersionRepositoryMock().Object, new MemoryCache(new MemoryCacheOptions()), Options.Create(new CacheOptions()));

        var result = await sut.GetByIdAsync(id, CancellationToken.None);

        result.Should().Be(prompt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<IPromptRepository>();
        repository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Prompt?)null);
        var sut = new PromptService(repository.Object, CreateVersionRepositoryMock().Object, new MemoryCache(new MemoryCacheOptions()), Options.Create(new CacheOptions()));

        var result = await sut.GetByIdAsync(id, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetVersionsAsync_DelegatesToVersionRepository()
    {
        var promptId = Guid.NewGuid();
        var versions = new List<PromptVersion> { new() { Id = Guid.NewGuid(), PromptId = promptId, Content = "v", Version = 1, CreatedAt = DateTime.UtcNow } };
        var versionRepo = new Mock<IPromptVersionRepository>();
        versionRepo.Setup(v => v.GetByPromptIdAsync(promptId, It.IsAny<CancellationToken>())).ReturnsAsync(versions);
        var repository = new Mock<IPromptRepository>();
        var sut = new PromptService(repository.Object, versionRepo.Object, new MemoryCache(new MemoryCacheOptions()), Options.Create(new CacheOptions()));

        var result = await sut.GetVersionsAsync(promptId, CancellationToken.None);

        result.Should().BeEquivalentTo(versions);
        versionRepo.Verify(v => v.GetByPromptIdAsync(promptId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenSuccessful_UpdatesTagsAndInvalidatesCache()
    {
        var repository = new Mock<IPromptRepository>();
        repository.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpsertPromptRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });
        repository.Setup(r => r.SetTagsAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });

        var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set("prompt:search:version", 3);
        var service = new PromptService(repository.Object, CreateVersionRepositoryMock().Object, cache, Options.Create(new CacheOptions()));
        var request = new UpsertPromptRequest { Title = "t", Content = "c", TagIds = [Guid.NewGuid()] };

        var result = await service.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

        result.Success.Should().BeTrue();
        cache.Get<int>("prompt:search:version").Should().Be(4);
        repository.Verify(r => r.SetTagsAsync(It.IsAny<Guid>(), request.TagIds, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WhenCached_DoesNotCallRepositoryTwice()
    {
        var repository = new Mock<IPromptRepository>();
        repository.Setup(r => r.SearchAsync(It.IsAny<PromptSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResponse<Prompt> { Items = [], PageNumber = 1, PageSize = 10, TotalRows = 0 });

        var service = new PromptService(
            repository.Object,
            CreateVersionRepositoryMock().Object,
            new MemoryCache(new MemoryCacheOptions()),
            Options.Create(new CacheOptions { PromptSearchTtlSeconds = 60 }));

        var request = new PromptSearchRequest { Query = "q", PageNumber = 1, PageSize = 10 };
        _ = await service.SearchAsync(request, CancellationToken.None);
        _ = await service.SearchAsync(request, CancellationToken.None);

        repository.Verify(r => r.SearchAsync(It.IsAny<PromptSearchRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetTagsAsync_WhenSuccessful_InvalidatesSearchCache()
    {
        var repository = new Mock<IPromptRepository>();
        repository.Setup(r => r.SetTagsAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true });

        var cache = new MemoryCache(new MemoryCacheOptions());
        cache.Set("prompt:search:version", 7);
        var service = new PromptService(repository.Object, CreateVersionRepositoryMock().Object, cache, Options.Create(new CacheOptions()));

        var result = await service.SetTagsAsync(Guid.NewGuid(), [Guid.NewGuid()], CancellationToken.None);

        result.Success.Should().BeTrue();
        cache.Get<int>("prompt:search:version").Should().Be(8);
    }
}
