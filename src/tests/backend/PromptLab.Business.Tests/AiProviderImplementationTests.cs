using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PromptLab.Business.Ai.Anthropic;
using PromptLab.Business.Ai.Contracts;
using PromptLab.Business.Ai.Google;
using PromptLab.Business.Ai.OpenAi;
using PromptLab.Business.Configuration;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Tests;

public class AiProviderImplementationTests
{
    private static Mock<IOptionsMonitor<AiOptions>> CreateMonitor(AiOptions options)
    {
        var m = new Mock<IOptionsMonitor<AiOptions>>();
        m.Setup(x => x.CurrentValue).Returns(options);
        return m;
    }

    private static Prompt BuildPrompt() =>
        new()
        {
            Id = Guid.NewGuid(),
            Title = "T",
            Content = "System instructions",
            IsActive = true
        };

    private static AnalyzeExecutionRequest BuildRequest(string provider = "openai", string modelId = "gpt-test") =>
        new()
        {
            Provider = provider,
            ModelId = modelId,
            Input = "hello",
            EffectiveSettings = new GenerationSettings
            {
                Temperature = 0.4m,
                MaxTokens = 64,
                TopP = 0.95m
            }
        };

    [Fact]
    public async Task GptAiProvider_WhenMock_ReturnsMockPrefix()
    {
        var client = new Mock<IGptAiProviderRequestClient>();
        client
            .Setup(c => c.AnalyzeAsync(It.IsAny<Prompt>(), It.IsAny<AnalyzeExecutionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnalyzeExecutionResult
            {
                Output = "[MOCK:openai] hi",
                LatencyMs = 1,
                Status = "Completed"
            });
        var provider = new GptAiProvider(client.Object);

        var result = await provider.AnalyzeAsync(BuildPrompt(), BuildRequest(), CancellationToken.None);

        result.Status.Should().Be("Completed");
        result.Output.Should().StartWith("[MOCK:openai]");
    }

    [Fact]
    public async Task GptAiProvider_WhenNotMockAndMissingKey_ReturnsFailed()
    {
        var client = new Mock<IGptAiProviderRequestClient>();
        client
            .Setup(c => c.AnalyzeAsync(It.IsAny<Prompt>(), It.IsAny<AnalyzeExecutionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnalyzeExecutionResult
            {
                Output = string.Empty,
                LatencyMs = 1,
                Status = "Failed",
                ErrorMessage = "API key not configured"
            });
        var provider = new GptAiProvider(client.Object);

        var result = await provider.AnalyzeAsync(BuildPrompt(), BuildRequest(), CancellationToken.None);

        result.Status.Should().Be("Failed");
        result.ErrorMessage.Should().Contain("API key");
    }

    [Fact]
    public async Task GptAiProviderRequestClient_ParsesOutputText_AndSendsBearer()
    {
        HttpRequestMessage? captured = null;
        var handler = new StubHttpMessageHandler(req =>
        {
            captured = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"output_text\":\"hi-openai\"}", Encoding.UTF8, "application/json")
            };
        });
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://api.openai.com/") };
        var options = new AiOptions
        {
            Providers = new AiProvidersConnectionOptions
            {
                OpenAi = new AiProviderConnectionOptions { ApiKey = "sk-test" }
            }
        };
        var client = new GptAiProviderRequestClient(
            new StubHttpClientFactory(http),
            CreateMonitor(options).Object,
            NullLogger<GptAiProviderRequestClient>.Instance);

        var result = await client.AnalyzeAsync(BuildPrompt(), BuildRequest(), CancellationToken.None);

        result.Status.Should().Be("Completed");
        result.Output.Should().Be("hi-openai");
        captured.Should().NotBeNull();
        captured!.Headers.Authorization!.Scheme.Should().Be("Bearer");
        captured.Headers.Authorization.Parameter.Should().Be("sk-test");
        captured.RequestUri!.ToString().Should().Contain("v1/responses");
    }

    [Fact]
    public async Task ClaudeAiProvider_WhenMock_ReturnsMockPrefix()
    {
        var client = new Mock<IClaudeAiProviderRequestClient>();
        client
            .Setup(c => c.AnalyzeAsync(It.IsAny<Prompt>(), It.IsAny<AnalyzeExecutionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnalyzeExecutionResult
            {
                Output = "[MOCK:anthropic] hi",
                LatencyMs = 1,
                Status = "Completed"
            });
        var req = BuildRequest("anthropic", "claude-test");
        var provider = new ClaudeAiProvider(client.Object);

        var result = await provider.AnalyzeAsync(BuildPrompt(), req, CancellationToken.None);

        result.Output.Should().StartWith("[MOCK:anthropic]");
    }

    [Fact]
    public async Task ClaudeAiProviderRequestClient_SendsHeaders_AndParsesContent()
    {
        HttpRequestMessage? captured = null;
        var handler = new StubHttpMessageHandler(req =>
        {
            captured = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"content\":[{\"type\":\"text\",\"text\":\"hi-claude\"}]}",
                    Encoding.UTF8,
                    "application/json")
            };
        });
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://api.anthropic.com/") };
        var options = new AiOptions
        {
            Providers = new AiProvidersConnectionOptions
            {
                Anthropic = new AiProviderConnectionOptions { ApiKey = "anthropic-key", AnthropicApiVersion = "2023-06-01" }
            }
        };
        var client = new ClaudeAiProviderRequestClient(
            new StubHttpClientFactory(http),
            CreateMonitor(options).Object,
            NullLogger<ClaudeAiProviderRequestClient>.Instance);
        var req = BuildRequest("anthropic", "claude-test");

        var result = await client.AnalyzeAsync(BuildPrompt(), req, CancellationToken.None);

        result.Output.Should().Be("hi-claude");
        captured.Should().NotBeNull();
        captured!.Headers.GetValues("x-api-key").Single().Should().Be("anthropic-key");
        captured.Headers.GetValues("anthropic-version").Single().Should().Be("2023-06-01");
        captured.RequestUri!.ToString().Should().Contain("v1/messages");
    }

    [Fact]
    public async Task GeminiAiProvider_WhenMock_ReturnsMockPrefix()
    {
        var client = new Mock<IGeminiAiProviderRequestClient>();
        client
            .Setup(c => c.AnalyzeAsync(It.IsAny<Prompt>(), It.IsAny<AnalyzeExecutionRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AnalyzeExecutionResult
            {
                Output = "[MOCK:google] hi",
                LatencyMs = 1,
                Status = "Completed"
            });
        var req = BuildRequest("google", "gemini-test");
        var provider = new GeminiAiProvider(client.Object);

        var result = await provider.AnalyzeAsync(BuildPrompt(), req, CancellationToken.None);

        result.Output.Should().StartWith("[MOCK:google]");
    }

    [Fact]
    public async Task GeminiAiProviderRequestClient_UsesGenerateContentUrl_AndParsesCandidates()
    {
        HttpRequestMessage? captured = null;
        var handler = new StubHttpMessageHandler(req =>
        {
            captured = req;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"candidates\":[{\"content\":{\"parts\":[{\"text\":\"hi-gemini\"}]}}]}",
                    Encoding.UTF8,
                    "application/json")
            };
        });
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://generativelanguage.googleapis.com/") };
        var options = new AiOptions
        {
            Providers = new AiProvidersConnectionOptions
            {
                Google = new AiProviderConnectionOptions { ApiKey = "google-key" }
            }
        };
        var client = new GeminiAiProviderRequestClient(
            new StubHttpClientFactory(http),
            CreateMonitor(options).Object,
            NullLogger<GeminiAiProviderRequestClient>.Instance);
        var req = BuildRequest("google", "gemini-2.5-pro");

        var result = await client.AnalyzeAsync(BuildPrompt(), req, CancellationToken.None);

        result.Output.Should().Be("hi-gemini");
        captured.Should().NotBeNull();
        var uri = captured!.RequestUri!.ToString();
        uri.Should().Contain("v1beta/models/");
        uri.Should().Contain("generateContent");
        uri.Should().Contain("key=google-key");
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        internal StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) =>
            _handler = handler;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(_handler(request));
    }

    private sealed class StubHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }
}
