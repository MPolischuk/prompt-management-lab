using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PromptLab.Entities.Analyze;

namespace PromptLab.Business.Ai;

internal static class AiHttpExecutionPipeline
{
    internal static async Task<AnalyzeExecutionResult> ExecuteAsync(
        HttpClient client,
        HttpRequestMessage request,
        Func<JsonElement, string?> textExtractor,
        ILogger logger,
        string providerName,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            sw.Stop();

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("{Provider} request failed: {Status} {Body}", providerName, (int)response.StatusCode, raw);
                return AiExecutionFailures.FromHttpError((int)response.StatusCode, raw, (int)sw.ElapsedMilliseconds);
            }

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;
            var text = textExtractor(root);
            if (string.IsNullOrEmpty(text))
            {
                var err = JsonResponseExtractors.TryGetErrorMessage(root);
                return new AnalyzeExecutionResult
                {
                    Output = string.Empty,
                    LatencyMs = (int)sw.ElapsedMilliseconds,
                    Status = "Failed",
                    ErrorMessage = string.IsNullOrEmpty(err)
                        ? $"Could not parse {providerName} response body."
                        : err
                };
            }

            return new AnalyzeExecutionResult
            {
                Output = text,
                LatencyMs = (int)sw.ElapsedMilliseconds,
                Status = "Completed"
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "{Provider} request threw", providerName);
            return AiExecutionFailures.FromException(ex, (int)sw.ElapsedMilliseconds);
        }
    }
}
