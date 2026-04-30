using System.Text.Json;

namespace PromptLab.Business.Ai;

internal static class JsonResponseExtractors
{
    internal static string? TryGetOpenAiText(JsonElement root)
    {
        if (root.TryGetProperty("output_text", out var outputText) && outputText.ValueKind == JsonValueKind.String)
            return outputText.GetString();

        if (root.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in output.EnumerateArray())
            {
                var t = ExtractFromOutputItem(item);
                if (!string.IsNullOrEmpty(t))
                    return t;
            }
        }

        // Chat Completions shape (fallback)
        if (root.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array)
        {
            foreach (var choice in choices.EnumerateArray())
            {
                if (choice.TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var content) &&
                    content.ValueKind == JsonValueKind.String)
                    return content.GetString();
            }
        }

        return null;
    }

    private static string? ExtractFromOutputItem(JsonElement item)
    {
        if (item.TryGetProperty("content", out var content))
        {
            if (content.ValueKind == JsonValueKind.Array)
            {
                foreach (var part in content.EnumerateArray())
                {
                    if (part.TryGetProperty("text", out var text) && text.ValueKind == JsonValueKind.String)
                        return text.GetString();
                }
            }
            else if (content.ValueKind == JsonValueKind.String)
                return content.GetString();
        }

        if (item.TryGetProperty("text", out var directText) && directText.ValueKind == JsonValueKind.String)
            return directText.GetString();

        return null;
    }

    internal static string? TryGetAnthropicText(JsonElement root)
    {
        if (!root.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
            return null;

        foreach (var block in content.EnumerateArray())
        {
            if (block.TryGetProperty("text", out var text) && text.ValueKind == JsonValueKind.String)
                return text.GetString();
        }

        return null;
    }

    internal static string? TryGetGeminiText(JsonElement root)
    {
        if (!root.TryGetProperty("candidates", out var candidates) || candidates.ValueKind != JsonValueKind.Array)
            return null;

        foreach (var candidate in candidates.EnumerateArray())
        {
            if (!candidate.TryGetProperty("content", out var content))
                continue;
            if (!content.TryGetProperty("parts", out var parts) || parts.ValueKind != JsonValueKind.Array)
                continue;
            foreach (var part in parts.EnumerateArray())
            {
                if (part.TryGetProperty("text", out var text) && text.ValueKind == JsonValueKind.String)
                    return text.GetString();
            }
        }

        return null;
    }

    internal static string? TryGetErrorMessage(JsonElement root)
    {
        if (root.TryGetProperty("error", out var error))
        {
            if (error.ValueKind == JsonValueKind.String)
                return error.GetString();
            if (error.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String)
                return msg.GetString();
        }

        return null;
    }
}
