using FluentAssertions;
using PromptLab.Business.Ai;

namespace PromptLab.Integrations.Tests.Ai;

public class AiExecutionFailuresTests
{
    [Fact]
    public void MissingApiKey_ContainsExpectedMessage()
    {
        var r = AiExecutionFailures.MissingApiKey("openai");

        r.Status.Should().Be("Failed");
        r.Output.Should().BeEmpty();
        r.ErrorMessage.Should().Contain("openai");
        r.ErrorMessage.Should().Contain("API key not configured");
    }

    [Fact]
    public void InvalidConfiguration_ContainsExpectedMessage()
    {
        var r = AiExecutionFailures.InvalidConfiguration("bad wiring");

        r.Status.Should().Be("Failed");
        r.ErrorMessage.Should().Be("bad wiring");
    }

    [Fact]
    public void FromHttpError_WithLongBody_TruncatesTo2000Chars()
    {
        var body = new string('x', 2500);
        var r = AiExecutionFailures.FromHttpError(500, body, 10);

        r.ErrorMessage.Should().Contain("HTTP 500");
        r.ErrorMessage.Should().EndWith("…");
        r.ErrorMessage.Length.Should().BeLessThan(body.Length + 30);
    }

    [Fact]
    public void FromException_IncludesExceptionMessage()
    {
        var r = AiExecutionFailures.FromException(new InvalidOperationException("boom"), 5);

        r.ErrorMessage.Should().Be("boom");
        r.LatencyMs.Should().Be(5);
    }
}
