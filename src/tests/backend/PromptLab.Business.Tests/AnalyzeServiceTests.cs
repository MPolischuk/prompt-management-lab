using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using PromptLab.Business.Ai;
using PromptLab.Business.Ai.Contracts;
using PromptLab.Business.Configuration;
using PromptLab.Business.Services;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;
using PromptLab.Entities.Contracts;
using PromptLab.Entities.Prompts;
using System.Security.Cryptography;
using System.Text;

namespace PromptLab.Business.Tests;

public class AnalyzeServiceTests
{
    private static IAiProvider CreateProvider(string name = "simulated")
    {
        var provider = new Mock<IAiProvider>();
        provider.SetupGet(p => p.Name).Returns(name);
        provider.Setup(p => p.AnalyzeAsync(It.IsAny<Prompt>(), It.IsAny<AnalyzeExecutionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnalyzeExecutionResult
            {
                Output = "ok",
                Status = "Completed",
                LatencyMs = 1
            });
        return provider.Object;
    }

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
                TargetModelId = "simulated-default",
                Temperature = 0.3m
            });

        var analyzeRepository = new Mock<IAnalyzeRepository>();
        analyzeRepository.Setup(r => r.CreateRunAsync(It.IsAny<AnalyzeRun>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });

        var aiOptions = BuildAiOptions();
        var providerFactory = new AiProviderFactory([CreateProvider()], Options.Create(aiOptions));
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
        var providerFactory = new AiProviderFactory([CreateProvider()], Options.Create(aiOptions));
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

    [Fact]
    public async Task AnalyzeAsync_WhenProviderIsNotRegistered_ReturnsUnavailableError()
    {
        var promptRepository = new Mock<IPromptRepository>();
        promptRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Prompt { Id = Guid.NewGuid(), Title = "Prompt", Content = "Content", IsActive = true });

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
        result.ErrorCode.Should().Be(OperationErrorCode.Unavailable);
        result.Message.Should().Be("Provider not configured.");
    }

    [Fact]
    public async Task AnalyzeAsync_UsesPromptTargetModel_WhenRequestModelIsMissing()
    {
        var promptId = Guid.NewGuid();
        var promptRepository = new Mock<IPromptRepository>();
        promptRepository.Setup(r => r.GetByIdAsync(promptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Prompt
            {
                Id = promptId,
                Title = "Prompt",
                Content = "Content",
                IsActive = true,
                TargetModelId = "simulated-default"
            });

        var captured = default(AnalyzeRun);
        var analyzeRepository = new Mock<IAnalyzeRepository>();
        analyzeRepository.Setup(r => r.CreateRunAsync(It.IsAny<AnalyzeRun>(), It.IsAny<CancellationToken>()))
            .Callback<AnalyzeRun, CancellationToken>((run, _) => captured = run)
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });

        var provider = CreateProvider();
        var aiOptions = BuildAiOptions();
        var service = new AnalyzeService(
            promptRepository.Object,
            analyzeRepository.Object,
            new AiProviderFactory([provider], Options.Create(aiOptions)),
            Options.Create(aiOptions),
            Options.Create(new CacheOptions()),
            new MemoryCache(new MemoryCacheOptions()));

        var result = await service.AnalyzeAsync(new AnalyzeRequest { PromptId = promptId, Input = "input" }, CancellationToken.None);

        result.Success.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.ModelId.Should().Be("simulated-default");
    }

    [Fact]
    public async Task AnalyzeAsync_UsesPromptModelHint_WhenTargetModelIsMissing()
    {
        var promptId = Guid.NewGuid();
        var promptRepository = new Mock<IPromptRepository>();
        promptRepository.Setup(r => r.GetByIdAsync(promptId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Prompt
            {
                Id = promptId,
                Title = "Prompt",
                Content = "Content",
                IsActive = true,
                ModelHint = "simulated-default"
            });

        var captured = default(AnalyzeRun);
        var analyzeRepository = new Mock<IAnalyzeRepository>();
        analyzeRepository.Setup(r => r.CreateRunAsync(It.IsAny<AnalyzeRun>(), It.IsAny<CancellationToken>()))
            .Callback<AnalyzeRun, CancellationToken>((run, _) => captured = run)
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });

        var provider = CreateProvider();
        var aiOptions = BuildAiOptions();
        var service = new AnalyzeService(
            promptRepository.Object,
            analyzeRepository.Object,
            new AiProviderFactory([provider], Options.Create(aiOptions)),
            Options.Create(aiOptions),
            Options.Create(new CacheOptions()),
            new MemoryCache(new MemoryCacheOptions()));

        var result = await service.AnalyzeAsync(new AnalyzeRequest { PromptId = promptId, Input = "input" }, CancellationToken.None);

        result.Success.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.ModelId.Should().Be("simulated-default");
    }

    [Fact]
    public async Task AnalyzeAsync_PersistsPromptHashAndEffectiveSettings()
    {
        var promptId = Guid.NewGuid();
        var prompt = new Prompt
        {
            Id = promptId,
            Title = "Prompt",
            Content = "Content",
            IsActive = true,
            TargetModelId = "simulated-default",
            Temperature = 0.3m,
            MaxTokens = 250,
            TopP = 0.8m
        };

        var promptRepository = new Mock<IPromptRepository>();
        promptRepository.Setup(r => r.GetByIdAsync(promptId, It.IsAny<CancellationToken>())).ReturnsAsync(prompt);

        var captured = default(AnalyzeRun);
        var analyzeRepository = new Mock<IAnalyzeRepository>();
        analyzeRepository.Setup(r => r.CreateRunAsync(It.IsAny<AnalyzeRun>(), It.IsAny<CancellationToken>()))
            .Callback<AnalyzeRun, CancellationToken>((run, _) => captured = run)
            .ReturnsAsync(new OperationResult { Success = true, EntityId = Guid.NewGuid() });

        var aiOptions = BuildAiOptions();
        var service = new AnalyzeService(
            promptRepository.Object,
            analyzeRepository.Object,
            new AiProviderFactory([CreateProvider()], Options.Create(aiOptions)),
            Options.Create(aiOptions),
            Options.Create(new CacheOptions()),
            new MemoryCache(new MemoryCacheOptions()));

        var result = await service.AnalyzeAsync(
            new AnalyzeRequest
            {
                PromptId = promptId,
                Input = "input",
                Settings = new GenerationSettings { Temperature = 0.9m, MaxTokens = 999, TopP = 0.4m }
            },
            CancellationToken.None);

        var expectedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(prompt.Content)));
        result.Success.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.PromptSnapshotHash.Should().Be(expectedHash);
        captured.Temperature.Should().Be(0.9m);
        captured.MaxTokens.Should().Be(999);
        captured.TopP.Should().Be(0.4m);
    }

    [Fact]
    public async Task GetProvidersAsync_UsesCache_AndCatalogProviders()
    {
        var aiOptions = BuildAiOptions();
        aiOptions = new AiOptions
        {
            DefaultProvider = aiOptions.DefaultProvider,
            Models = aiOptions.Models,
            EnabledProviders = ["simulated"],
            CatalogProviders = ["simulated", "openai"]
        };

        var service = new AnalyzeService(
            Mock.Of<IPromptRepository>(),
            Mock.Of<IAnalyzeRepository>(),
            new AiProviderFactory([CreateProvider()], Options.Create(aiOptions)),
            Options.Create(aiOptions),
            Options.Create(new CacheOptions { ProvidersTtlSeconds = 60 }),
            new MemoryCache(new MemoryCacheOptions()));

        var first = await service.GetProvidersAsync(CancellationToken.None);
        var second = await service.GetProvidersAsync(CancellationToken.None);

        ReferenceEquals(first, second).Should().BeTrue();
        first.Select(p => p.Name).Should().Contain(["simulated", "openai"]);
        first.Single(p => p.Name == "openai").Enabled.Should().BeFalse();
    }

    [Fact]
    public async Task GetModelsAsync_UsesCache()
    {
        var aiOptions = BuildAiOptions();
        var service = new AnalyzeService(
            Mock.Of<IPromptRepository>(),
            Mock.Of<IAnalyzeRepository>(),
            new AiProviderFactory([CreateProvider()], Options.Create(aiOptions)),
            Options.Create(aiOptions),
            Options.Create(new CacheOptions { ProvidersTtlSeconds = 60 }),
            new MemoryCache(new MemoryCacheOptions()));

        var first = await service.GetModelsAsync(CancellationToken.None);
        var second = await service.GetModelsAsync(CancellationToken.None);

        ReferenceEquals(first, second).Should().BeTrue();
    }

    [Fact]
    public async Task GetProvidersAsync_WhenVendorProviderRegistered_MarksCatalogEntryEnabled()
    {
        var aiOptions = new AiOptions
        {
            DefaultProvider = "simulated",
            EnabledProviders = ["simulated", "openai"],
            CatalogProviders = ["simulated", "openai"],
            Models =
            [
                new AiModelOption
                {
                    Id = "simulated-default",
                    Provider = "simulated",
                    DisplayName = "Simulated Default",
                    Enabled = true
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

        var openAi = new Mock<IAiProvider>();
        openAi.Setup(p => p.Name).Returns("openai");

        var service = new AnalyzeService(
            Mock.Of<IPromptRepository>(),
            Mock.Of<IAnalyzeRepository>(),
            new AiProviderFactory([CreateProvider(), openAi.Object], Options.Create(aiOptions)),
            Options.Create(aiOptions),
            Options.Create(new CacheOptions { ProvidersTtlSeconds = 60 }),
            new MemoryCache(new MemoryCacheOptions()));

        var providers = await service.GetProvidersAsync(CancellationToken.None);

        providers.Single(p => p.Name == "openai").Enabled.Should().BeTrue();
        providers.Single(p => p.Name == "simulated").Enabled.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WhenRunMissing_ReturnsNull()
    {
        var id = Guid.NewGuid();
        var analyzeRepository = new Mock<IAnalyzeRepository>();
        analyzeRepository.Setup(r => r.GetRunByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AnalyzeRun?)null);
        var aiOptions = BuildAiOptions();
        var service = new AnalyzeService(
            Mock.Of<IPromptRepository>(),
            analyzeRepository.Object,
            new AiProviderFactory([], Options.Create(aiOptions)),
            Options.Create(aiOptions),
            Options.Create(new CacheOptions()),
            new MemoryCache(new MemoryCacheOptions()));

        var result = await service.GetByIdAsync(id, CancellationToken.None);

        result.Should().BeNull();
        analyzeRepository.Verify(r => r.GetRunByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
