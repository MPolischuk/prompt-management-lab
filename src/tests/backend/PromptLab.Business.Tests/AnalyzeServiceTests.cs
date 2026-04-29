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
    private static AiOptions BuildAiOptions()
    {
        return new AiOptions
        {
            DefaultProvider = "simulated",
            EnabledProviders = ["simulated", "openai"],
            Models =
            [
                new AiModelOption
                {
                    Id = "simulated-default",
                    Provider = "simulated",
                    DisplayName = "Simulated Default",
                    Enabled = true,
                    DefaultTemperature = 0.7m,
                    DefaultMaxTokens = 400,
                    DefaultTopP = 1.0m
                },
                new AiModelOption
                {
                    Id = "gpt-5.5",
                    Provider = "openai",
                    DisplayName = "GPT-5.5",
                    Enabled = true
                }
            ]
        };
    }

    [Fact]
    public async Task AnalyzeAsync_WhenPromptDoesNotExist_ReturnsFailure()
    {
        var promptRepository = new Mock<IPromptRepository>();
        promptRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Prompt?)null);

        var analyzeRepository = new Mock<IAnalyzeRepository>();
        var aiOptions = BuildAiOptions();
        var providerFactory = new AiProviderFactory([], Options.Create(aiOptions));
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var service = new AnalyzeService(
            promptRepository.Object,
            analyzeRepository.Object,
            providerFactory,
            Options.Create(aiOptions),
            Options.Create(new CacheOptions()),
            memoryCache);

        var result = await service.AnalyzeAsync(
            new AnalyzeRequest { PromptId = Guid.NewGuid(), Provider = "simulated", ModelId = "simulated-default", Input = "input" },
            CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Prompt not found.");
    }

    [Fact]
    public async Task AnalyzeAsync_WhenProviderExists_PersistsRun()
    {
        var promptRepository = new Mock<IPromptRepository>();
        promptRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Prompt
            {
                Id = Guid.NewGuid(),
                Title = "Prompt",
                Content = "Content",
                IsActive = true,
                DefaultModelId = "simulated-default",
                Temperature = 0.3m
            });

        var analyzeRepository = new Mock<IAnalyzeRepository>();
        analyzeRepository.Setup(r => r.CreateRunAsync(It.IsAny<AnalyzeRun>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });

        var aiOptions = BuildAiOptions();
        var providerFactory = new AiProviderFactory([new SimulatedAiProvider()], Options.Create(aiOptions));
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var service = new AnalyzeService(
            promptRepository.Object,
            analyzeRepository.Object,
            providerFactory,
            Options.Create(aiOptions),
            Options.Create(new CacheOptions()),
            memoryCache);

        var result = await service.AnalyzeAsync(
            new AnalyzeRequest { PromptId = Guid.NewGuid(), Provider = "simulated", ModelId = "simulated-default", Input = "input" },
            CancellationToken.None);

        result.Success.Should().BeTrue();
        analyzeRepository.Verify(r => r.CreateRunAsync(It.IsAny<AnalyzeRun>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AnalyzeAsync_WhenModelDoesNotBelongToProvider_ReturnsFailure()
    {
        var promptRepository = new Mock<IPromptRepository>();
        promptRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Prompt { Id = Guid.NewGuid(), Title = "Prompt", Content = "Content", IsActive = true });

        var analyzeRepository = new Mock<IAnalyzeRepository>();
        var aiOptions = BuildAiOptions();
        var providerFactory = new AiProviderFactory([new SimulatedAiProvider()], Options.Create(aiOptions));
        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var service = new AnalyzeService(
            promptRepository.Object,
            analyzeRepository.Object,
            providerFactory,
            Options.Create(aiOptions),
            Options.Create(new CacheOptions()),
            memoryCache);

        var result = await service.AnalyzeAsync(
            new AnalyzeRequest { PromptId = Guid.NewGuid(), Provider = "simulated", ModelId = "gpt-5.5", Input = "input" },
            CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Model does not belong to selected provider.");
    }
}
