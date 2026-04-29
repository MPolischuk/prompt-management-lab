using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using PromptLab.Business.Ai;
using PromptLab.Business.Configuration;
using PromptLab.Business.Services;
using PromptLab.Data.Repositories;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Tests;

public class AnalyzeServiceTests
{
    [Fact]
    public async Task AnalyzeAsync_WhenPromptDoesNotExist_ReturnsFailure()
    {
        var promptRepository = new Mock<IPromptRepository>();
        promptRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Prompt?)null);

        var analyzeRepository = new Mock<IAnalyzeRepository>();
        var providerFactory = new AiProviderFactory([], Options.Create(new AiOptions()));
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var service = new AnalyzeService(
            promptRepository.Object,
            analyzeRepository.Object,
            providerFactory,
            Options.Create(new AiOptions()),
            Options.Create(new CacheOptions()),
            memoryCache);

        var result = await service.AnalyzeAsync(
            new AnalyzeRequest { PromptId = Guid.NewGuid(), Provider = "simulated", Input = "input" },
            CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Prompt not found.");
    }

    [Fact]
    public async Task AnalyzeAsync_WhenProviderExists_PersistsRun()
    {
        var promptRepository = new Mock<IPromptRepository>();
        promptRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Prompt { Id = Guid.NewGuid(), Title = "Prompt", Content = "Content", IsActive = true });

        var analyzeRepository = new Mock<IAnalyzeRepository>();
        analyzeRepository.Setup(r => r.CreateRunAsync(It.IsAny<AnalyzeRun>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });

        var providerFactory = new AiProviderFactory([new SimulatedAiProvider()], Options.Create(new AiOptions()));
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var service = new AnalyzeService(
            promptRepository.Object,
            analyzeRepository.Object,
            providerFactory,
            Options.Create(new AiOptions()),
            Options.Create(new CacheOptions()),
            memoryCache);

        var result = await service.AnalyzeAsync(
            new AnalyzeRequest { PromptId = Guid.NewGuid(), Provider = "simulated", Input = "input" },
            CancellationToken.None);

        result.Success.Should().BeTrue();
        analyzeRepository.Verify(r => r.CreateRunAsync(It.IsAny<AnalyzeRun>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
