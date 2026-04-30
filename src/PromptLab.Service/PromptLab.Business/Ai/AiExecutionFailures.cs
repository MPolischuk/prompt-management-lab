using PromptLab.Entities.Analyze;

namespace PromptLab.Business.Ai;

internal static class AiExecutionFailures
{
    internal static AnalyzeExecutionResult MissingApiKey(string providerName, int latencyMs = 0) =>
        new()
        {
            Output = string.Empty,
            LatencyMs = latencyMs,
            Status = "Failed",
            ErrorMessage = $"API key not configured for provider '{providerName}'. Set Ai:Providers:*:ApiKey or use Mock=true."
        };

    internal static AnalyzeExecutionResult InvalidConfiguration(string message, int latencyMs = 0) =>
        new()
        {
            Output = string.Empty,
            LatencyMs = latencyMs,
            Status = "Failed",
            ErrorMessage = message
        };

    internal static AnalyzeExecutionResult FromHttpError(int statusCode, string body, int latencyMs) =>
        new()
        {
            Output = string.Empty,
            LatencyMs = latencyMs,
            Status = "Failed",
            ErrorMessage = $"HTTP {(int)statusCode}: {Truncate(body, 2000)}"
        };

    internal static AnalyzeExecutionResult FromException(Exception ex, int latencyMs) =>
        new()
        {
            Output = string.Empty,
            LatencyMs = latencyMs,
            Status = "Failed",
            ErrorMessage = ex.Message
        };

    private static string Truncate(string value, int max)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= max)
            return value;
        return value[..max] + "…";
    }
}
