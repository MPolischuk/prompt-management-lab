using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using PromptLab.Business.Ai;

namespace PromptLab.Integrations.Tests.Ai;

public class AiHttpExecutionPipelineTests
{
    [Fact]
    public async Task ExecuteAsync_WhenSuccess_ReturnsCompletedWithOutput()
    {
        using var client = new HttpClient(new OkHandler());

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://local/");
        var logger = NullLogger.Instance;

        var result = await AiHttpExecutionPipeline.ExecuteAsync(
            client,
            request,
            JsonResponseExtractors.TryGetOpenAiText,
            logger,
            "Test",
            CancellationToken.None);

        result.Status.Should().Be("Completed");
        result.Output.Should().Be("hi");
        result.LatencyMs.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task ExecuteAsync_WhenHttpError_ReturnsFailedWithHttpPrefix()
    {
        using var client = new HttpClient(new StatusCodeHandler(HttpStatusCode.TooManyRequests, "slow down"));

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://local/");
        var logger = NullLogger.Instance;

        var result = await AiHttpExecutionPipeline.ExecuteAsync(
            client,
            request,
            JsonResponseExtractors.TryGetOpenAiText,
            logger,
            "Test",
            CancellationToken.None);

        result.Status.Should().Be("Failed");
        result.ErrorMessage.Should().Contain("HTTP 429");
        result.ErrorMessage.Should().Contain("slow down");
    }

    [Fact]
    public async Task ExecuteAsync_WhenSuccessButNoText_ReturnsFailedParse()
    {
        using var client = new HttpClient(new OkHandler("""{"x":1}"""));

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://local/");
        var logger = NullLogger.Instance;

        var result = await AiHttpExecutionPipeline.ExecuteAsync(
            client,
            request,
            JsonResponseExtractors.TryGetOpenAiText,
            logger,
            "Test",
            CancellationToken.None);

        result.Status.Should().Be("Failed");
        result.ErrorMessage.Should().Contain("parse");
    }

    [Fact]
    public async Task ExecuteAsync_WhenCanceled_PropagatesCancellation()
    {
        using var client = new HttpClient(new CancellingHandler());

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://local/");
        var logger = NullLogger.Instance;
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = async () => await AiHttpExecutionPipeline.ExecuteAsync(
            client,
            request,
            JsonResponseExtractors.TryGetOpenAiText,
            logger,
            "Test",
            cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenSendThrows_ReturnsFailedWithExceptionMessage()
    {
        using var client = new HttpClient(new ThrowingHandler());

        using var request = new HttpRequestMessage(HttpMethod.Get, "http://local/");
        var logger = NullLogger.Instance;

        var result = await AiHttpExecutionPipeline.ExecuteAsync(
            client,
            request,
            JsonResponseExtractors.TryGetOpenAiText,
            logger,
            "Test",
            CancellationToken.None);

        result.Status.Should().Be("Failed");
        result.ErrorMessage.Should().Contain("network boom");
    }

    private sealed class OkHandler : HttpMessageHandler
    {
        private readonly string _body;

        public OkHandler(string body = """{"output_text":"hi"}""")
        {
            _body = body;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var msg = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_body, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(msg);
        }
    }

    private sealed class StatusCodeHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _code;
        private readonly string _body;

        public StatusCodeHandler(HttpStatusCode code, string body)
        {
            _code = code;
            _body = body;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var msg = new HttpResponseMessage(_code) { Content = new StringContent(_body, Encoding.UTF8, "text/plain") };
            return Task.FromResult(msg);
        }
    }

    private sealed class ThrowingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            throw new IOException("network boom");
    }

    private sealed class CancellingHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromCanceled<HttpResponseMessage>(cancellationToken);
    }
}
