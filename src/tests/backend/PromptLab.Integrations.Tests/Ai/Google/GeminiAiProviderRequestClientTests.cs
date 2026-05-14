using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PromptLab.Business.Ai;
using PromptLab.Business.Ai.Google;
using PromptLab.Business.Configuration;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Integrations.Tests.Ai.Google;

public class GeminiAiProviderRequestClientTests
{
    private static AiOptions GoogleWithKey(string apiKey = "gemini-key") =>
        new()
        {
            DefaultProvider = "google",
            EnabledProviders = ["google"],
            Models = [],
            Providers = new AiProvidersConnectionOptions
            {
                Google = new AiProviderConnectionOptions { Enabled = true, Mock = false, ApiKey = apiKey }
            }
        };

    private static IOptionsMonitor<AiOptions> Monitor(AiOptions value)
    {
        var m = new Mock<IOptionsMonitor<AiOptions>>();
        m.Setup(x => x.CurrentValue).Returns(value);
        return m.Object;
    }

    private sealed class CaptureGeminiHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            LastBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            var json = """{"candidates":[{"content":{"parts":[{"text":"gemini-out"}]}}]}""";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }

    private static (GeminiAiProviderRequestClient Sut, CaptureGeminiHandler Handler) CreateSut(AiOptions options)
    {
        var handler = new CaptureGeminiHandler();
        var services = new ServiceCollection();
        services.AddHttpClient(AiHttpClientNames.Google, c => c.BaseAddress = new Uri("https://generativelanguage.googleapis.com/"))
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var sp = services.BuildServiceProvider();
        var factory = sp.GetRequiredService<IHttpClientFactory>();
        var sut = new GeminiAiProviderRequestClient(factory, Monitor(options), NullLogger<GeminiAiProviderRequestClient>.Instance);
        return (sut, handler);
    }

    private static Prompt Prompt() =>
        new() { Id = Guid.NewGuid(), Title = "T", Content = "C", IsActive = true };

    [Fact]
    public async Task AnalyzeAsync_WhenApiKeyEmpty_ReturnsMissingApiKey()
    {
        var options = GoogleWithKey("");
        var (sut, _) = CreateSut(options);

        var result = await sut.AnalyzeAsync(
            Prompt(),
            new AnalyzeExecutionRequest
            {
                Provider = "google",
                ModelId = "gemini-pro",
                EffectiveSettings = new GenerationSettings()
            },
            CancellationToken.None);

        result.Status.Should().Be("Failed");
        result.ErrorMessage.Should().Contain("google");
    }

    [Fact]
    public async Task AnalyzeAsync_BuildsCorrectUrl_WithEscapedModelAndKey()
    {
        var (sut, handler) = CreateSut(GoogleWithKey("k&x=1"));

        await sut.AnalyzeAsync(
            Prompt(),
            new AnalyzeExecutionRequest
            {
                Provider = "google",
                ModelId = "models/gemini-1.5",
                EffectiveSettings = new GenerationSettings()
            },
            CancellationToken.None);

        handler.LastRequest!.RequestUri.Should().NotBeNull();
        var uri = handler.LastRequest.RequestUri!;
        uri.AbsolutePath.Should().Contain("models%2Fgemini-1.5");
        uri.Query.Should().Contain(Uri.EscapeDataString("k&x=1"));
    }

    [Fact]
    public async Task AnalyzeAsync_OmitsGenerationConfigWhenNoParams()
    {
        var (sut, handler) = CreateSut(GoogleWithKey());

        await sut.AnalyzeAsync(
            Prompt(),
            new AnalyzeExecutionRequest
            {
                Provider = "google",
                ModelId = "gemini-pro",
                EffectiveSettings = new GenerationSettings()
            },
            CancellationToken.None);

        using var doc = JsonDocument.Parse(handler.LastBody!);
        doc.RootElement.TryGetProperty("generationConfig", out _).Should().BeFalse();
    }
}
