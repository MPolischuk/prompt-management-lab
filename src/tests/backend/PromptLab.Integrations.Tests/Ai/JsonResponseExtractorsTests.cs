using System.Text.Json;
using FluentAssertions;
using PromptLab.Business.Ai;

namespace PromptLab.Integrations.Tests.Ai;

public class JsonResponseExtractorsTests
{
    [Fact]
    public void TryGetOpenAiText_FromOutputText()
    {
        using var doc = JsonDocument.Parse("""{"output_text":"hello"}""");
        JsonResponseExtractors.TryGetOpenAiText(doc.RootElement).Should().Be("hello");
    }

    [Fact]
    public void TryGetOpenAiText_FromChoicesMessageContent()
    {
        using var doc = JsonDocument.Parse("""{"choices":[{"message":{"content":"x"}}]}""");
        JsonResponseExtractors.TryGetOpenAiText(doc.RootElement).Should().Be("x");
    }

    [Fact]
    public void TryGetOpenAiText_FromOutputArrayWithTextParts()
    {
        const string json = """{"output":[{"content":[{"text":"part"}]}]}""";
        using var doc = JsonDocument.Parse(json);
        JsonResponseExtractors.TryGetOpenAiText(doc.RootElement).Should().Be("part");
    }

    [Fact]
    public void TryGetOpenAiText_WhenMissing_ReturnsNull()
    {
        using var doc = JsonDocument.Parse("{}");
        JsonResponseExtractors.TryGetOpenAiText(doc.RootElement).Should().BeNull();
    }

    [Fact]
    public void TryGetAnthropicText_WhenPresent_ReturnsText()
    {
        using var doc = JsonDocument.Parse("""{"content":[{"text":"anth"}]}""");
        JsonResponseExtractors.TryGetAnthropicText(doc.RootElement).Should().Be("anth");
    }

    [Fact]
    public void TryGetAnthropicText_WhenMissing_ReturnsNull()
    {
        using var doc = JsonDocument.Parse("""{"content":[]}""");
        JsonResponseExtractors.TryGetAnthropicText(doc.RootElement).Should().BeNull();
    }

    [Fact]
    public void TryGetGeminiText_WhenPresent_ReturnsText()
    {
        using var doc = JsonDocument.Parse("""{"candidates":[{"content":{"parts":[{"text":"g"}]}}]}""");
        JsonResponseExtractors.TryGetGeminiText(doc.RootElement).Should().Be("g");
    }

    [Fact]
    public void TryGetGeminiText_WhenMissing_ReturnsNull()
    {
        using var doc = JsonDocument.Parse("""{"candidates":[]}""");
        JsonResponseExtractors.TryGetGeminiText(doc.RootElement).Should().BeNull();
    }

    [Fact]
    public void TryGetErrorMessage_FromString()
    {
        using var doc = JsonDocument.Parse("""{"error":"oops"}""");
        JsonResponseExtractors.TryGetErrorMessage(doc.RootElement).Should().Be("oops");
    }

    [Fact]
    public void TryGetErrorMessage_FromObjectMessage()
    {
        using var doc = JsonDocument.Parse("""{"error":{"message":"bad"}}""");
        JsonResponseExtractors.TryGetErrorMessage(doc.RootElement).Should().Be("bad");
    }

    [Fact]
    public void TryGetErrorMessage_WhenAbsent_ReturnsNull()
    {
        using var doc = JsonDocument.Parse("{}");
        JsonResponseExtractors.TryGetErrorMessage(doc.RootElement).Should().BeNull();
    }
}
