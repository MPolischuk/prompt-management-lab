using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PromptLab.Business.Ai;
using PromptLab.Business.Ai.Anthropic;
using PromptLab.Business.Configuration;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Integrations.Tests.Ai.Anthropic;

public class ClaudeAiProviderRequestClientTests
{
    private static AiOptions AnthropicWithKey(string apiKey = "sk-ant", string? anthropicVersion = null) =>
        new()
        {
            DefaultProvider = "anthropic",
            EnabledProviders = ["anthropic"],
            Models = [],
            Providers = new AiProvidersConnectionOptions
            {
                Anthropic = new AiProviderConnectionOptions
                {
                    Enabled = true,
                    Mock = false,
                    ApiKey = apiKey,
                    AnthropicApiVersion = anthropicVersion
                }
            }
        };

    private static IOptionsMonitor<AiOptions> Monitor(AiOptions value)
    {
        var m = new Mock<IOptionsMonitor<AiOptions>>();
        m.Setup(x => x.CurrentValue).Returns(value);
        return m.Object;
    }

    private sealed class CaptureAnthropicHandler : HttpMessageHandler
    {
        public string? LastBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            var json = """{"content":[{"text":"anthropic-out"}]}""";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }

    private static (ClaudeAiProviderRequestClient Sut, CaptureAnthropicHandler Handler) CreateSut(AiOptions options)
    {
        var handler = new CaptureAnthropicHandler();
        var services = new ServiceCollection();
        services.AddHttpClient(AiHttpClientNames.Anthropic, c => c.BaseAddress = new Uri("https://api.anthropic.com/"))
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var sp = services.BuildServiceProvider();
        var factory = sp.GetRequiredService<IHttpClientFactory>();
        var sut = new ClaudeAiProviderRequestClient(factory, Monitor(options), NullLogger<ClaudeAiProviderRequestClient>.Instance);
        return (sut, handler);
    }

    private static Prompt Prompt() =>
        new() { Id = Guid.NewGuid(), Title = "T", Content = "C", IsActive = true };

    [Fact]
    public async Task AnalyzeAsync_WhenApiKeyEmpty_ReturnsMissingApiKey()
    {
        var (sut, _) = CreateSut(AnthropicWithKey(""));

        var result = await sut.AnalyzeAsync(
            Prompt(),
            new AnalyzeExecutionRequest
            {
                Provider = "anthropic",
                ModelId = "claude-3",
                EffectiveSettings = new GenerationSettings()
            },
            CancellationToken.None);

        result.Status.Should().Be("Failed");
        result.ErrorMessage.Should().Contain("anthropic");
    }

    [Fact]
    public async Task AnalyzeAsync_UsesDefaultMaxTokens1024WhenNotSet()
    {
        var (sut, handler) = CreateSut(AnthropicWithKey());

        await sut.AnalyzeAsync(
            Prompt(),
            new AnalyzeExecutionRequest
            {
                Provider = "anthropic",
                ModelId = "claude-3",
                EffectiveSettings = new GenerationSettings { MaxTokens = null }
            },
            CancellationToken.None);

        using var doc = JsonDocument.Parse(handler.LastBody!);
        doc.RootElement.GetProperty("max_tokens").GetInt32().Should().Be(1024);
    }

    [Fact]
    public async Task AnalyzeAsync_UsesDefaultMaxTokens1024WhenZeroOrNegative()
    {
        var (sut, handler) = CreateSut(AnthropicWithKey());

        await sut.AnalyzeAsync(
            Prompt(),
            new AnalyzeExecutionRequest
            {
                Provider = "anthropic",
                ModelId = "claude-3",
                EffectiveSettings = new GenerationSettings { MaxTokens = 0 }
            },
            CancellationToken.None);

        using var doc = JsonDocument.Parse(handler.LastBody!);
        doc.RootElement.GetProperty("max_tokens").GetInt32().Should().Be(1024);
    }

    [Fact]
    public async Task AnalyzeAsync_SendsRequiredHeaders()
    {
        var handler2 = new CaptureRequestHandler();
        var services = new ServiceCollection();
        services.AddHttpClient(AiHttpClientNames.Anthropic, c => c.BaseAddress = new Uri("https://api.anthropic.com/"))
            .ConfigurePrimaryHttpMessageHandler(() => handler2);
        var sp = services.BuildServiceProvider();
        var factory = sp.GetRequiredService<IHttpClientFactory>();
        var sut2 = new ClaudeAiProviderRequestClient(factory, Monitor(AnthropicWithKey("my-key", "2024-01-01")), NullLogger<ClaudeAiProviderRequestClient>.Instance);
        await sut2.AnalyzeAsync(
            Prompt(),
            new AnalyzeExecutionRequest
            {
                Provider = "anthropic",
                ModelId = "claude-3",
                EffectiveSettings = new GenerationSettings()
            },
            CancellationToken.None);

        handler2.LastRequest!.Headers.GetValues("x-api-key").Single().Should().Be("my-key");
        handler2.LastRequest.Headers.GetValues("anthropic-version").Single().Should().Be("2024-01-01");
    }

    private sealed class CaptureRequestHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"content":[{"text":"x"}]}""", Encoding.UTF8, "application/json")
            });
        }
    }
}
