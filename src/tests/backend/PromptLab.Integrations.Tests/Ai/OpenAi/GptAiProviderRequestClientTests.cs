using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PromptLab.Business.Ai;
using PromptLab.Business.Ai.OpenAi;
using PromptLab.Business.Configuration;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Integrations.Tests.Ai.OpenAi;

public class GptAiProviderRequestClientTests
{
    private static AiOptions OpenAiWithKey(string apiKey = "sk-test") =>
        new()
        {
            DefaultProvider = "openai",
            EnabledProviders = ["openai"],
            Models = [],
            Providers = new AiProvidersConnectionOptions
            {
                OpenAi = new AiProviderConnectionOptions { Enabled = true, Mock = false, ApiKey = apiKey }
            }
        };

    private static IOptionsMonitor<AiOptions> Monitor(AiOptions value)
    {
        var m = new Mock<IOptionsMonitor<AiOptions>>();
        m.Setup(x => x.CurrentValue).Returns(value);
        return m.Object;
    }

    private sealed class CaptureOpenAiHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            LastBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            var json = JsonSerializer.Serialize(new Dictionary<string, object?> { ["output_text"] = "extracted" });
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }

    private static (GptAiProviderRequestClient Sut, CaptureOpenAiHandler Handler) CreateSut(AiOptions options)
    {
        var handler = new CaptureOpenAiHandler();
        var services = new ServiceCollection();
        services.AddHttpClient(AiHttpClientNames.OpenAi, c => c.BaseAddress = new Uri("https://api.openai.com/"))
            .ConfigurePrimaryHttpMessageHandler(() => handler);
        var sp = services.BuildServiceProvider();
        var factory = sp.GetRequiredService<IHttpClientFactory>();
        var sut = new GptAiProviderRequestClient(factory, Monitor(options), NullLogger<GptAiProviderRequestClient>.Instance);
        return (sut, handler);
    }

    private static Prompt Prompt() =>
        new()
        {
            Id = Guid.NewGuid(),
            Title = "T",
            Content = "C",
            IsActive = true
        };

    private static AnalyzeExecutionRequest Request() =>
        new()
        {
            Provider = "openai",
            ModelId = "gpt-4o-mini",
            Input = "hello",
            EffectiveSettings = new GenerationSettings
            {
                Temperature = 0.2m,
                TopP = 0.95m,
                MaxTokens = 128
            }
        };

    [Fact]
    public async Task AnalyzeAsync_WhenApiKeyEmpty_ReturnsMissingApiKey()
    {
        var options = OpenAiWithKey("");
        var (sut, _) = CreateSut(options);

        var result = await sut.AnalyzeAsync(Prompt(), Request(), CancellationToken.None);

        result.Status.Should().Be("Failed");
        result.ErrorMessage.Should().Contain("openai");
        result.ErrorMessage.Should().Contain("API key not configured");
    }

    [Fact]
    public async Task AnalyzeAsync_PostsToCorrectUrl()
    {
        var (sut, handler) = CreateSut(OpenAiWithKey());

        _ = await sut.AnalyzeAsync(Prompt(), Request(), CancellationToken.None);

        handler.LastRequest.Should().NotBeNull();
        handler.LastRequest!.RequestUri.Should().NotBeNull();
        handler.LastRequest.RequestUri!.AbsolutePath.Should().EndWith("/v1/responses");
    }

    [Fact]
    public async Task AnalyzeAsync_SendsAuthorizationBearerHeader()
    {
        var (sut, handler) = CreateSut(OpenAiWithKey("  secret-key  "));

        _ = await sut.AnalyzeAsync(Prompt(), Request(), CancellationToken.None);

        handler.LastRequest!.Headers.Authorization.Should().NotBeNull();
        handler.LastRequest.Headers.Authorization!.Scheme.Should().Be("Bearer");
        handler.LastRequest.Headers.Authorization.Parameter.Should().Be("secret-key");
    }

    [Fact]
    public async Task AnalyzeAsync_SendsCorrectJsonBody()
    {
        var (sut, handler) = CreateSut(OpenAiWithKey());

        _ = await sut.AnalyzeAsync(Prompt(), Request(), CancellationToken.None);

        handler.LastBody.Should().NotBeNull();
        using var doc = JsonDocument.Parse(handler.LastBody!);
        var root = doc.RootElement;
        root.GetProperty("model").GetString().Should().Be("gpt-4o-mini");
        root.GetProperty("input").GetString().Should().Contain("C");
        root.GetProperty("temperature").GetDouble().Should().BeApproximately(0.2, 0.0001);
        root.GetProperty("top_p").GetDouble().Should().BeApproximately(0.95, 0.0001);
        root.GetProperty("max_output_tokens").GetInt32().Should().Be(128);
    }
}
