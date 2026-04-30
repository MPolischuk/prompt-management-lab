using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PromptLab.Business.Configuration;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai.Anthropic;

/// <summary>
/// Cliente HTTP para Anthropic Messages API (<c>/v1/messages</c>).
/// </summary>
public class ClaudeAiProviderRequestClient(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<AiOptions> options,
    ILogger<ClaudeAiProviderRequestClient> logger) : IClaudeAiProviderRequestClient
{
    public async Task<AnalyzeExecutionResult> AnalyzeAsync(
        Prompt prompt,
        AnalyzeExecutionRequest request,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var connection = options.CurrentValue.Providers.Anthropic;
            if (string.IsNullOrWhiteSpace(connection.ApiKey))
                return AiExecutionFailures.MissingApiKey("anthropic");

            var client = httpClientFactory.CreateClient(AiHttpClientNames.Anthropic);
            var combined = AiPromptComposer.BuildUserFacingText(prompt, request);
            var settings = request.EffectiveSettings;

            var maxTokens = settings.MaxTokens is > 0 ? settings.MaxTokens!.Value : 1024;

            var messages = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = combined
                }
            };

            var body = new JsonObject
            {
                ["model"] = request.ModelId,
                ["max_tokens"] = maxTokens,
                ["messages"] = messages
            };

            if (settings.Temperature.HasValue)
                body["temperature"] = (double)settings.Temperature.Value;
            if (settings.TopP.HasValue)
                body["top_p"] = (double)settings.TopP.Value;

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/messages")
            {
                Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.TryAddWithoutValidation("x-api-key", connection.ApiKey!.Trim());
            var version = string.IsNullOrWhiteSpace(connection.AnthropicApiVersion)
                ? "2023-06-01"
                : connection.AnthropicApiVersion.Trim();
            httpRequest.Headers.TryAddWithoutValidation("anthropic-version", version);

            using var response = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            sw.Stop();

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Anthropic request failed: {Status} {Body}", (int)response.StatusCode, raw);
                return AiExecutionFailures.FromHttpError((int)response.StatusCode, raw, (int)sw.ElapsedMilliseconds);
            }

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;
            var text = JsonResponseExtractors.TryGetAnthropicText(root);
            if (string.IsNullOrEmpty(text))
            {
                var err = JsonResponseExtractors.TryGetErrorMessage(root);
                return new AnalyzeExecutionResult
                {
                    Output = string.Empty,
                    LatencyMs = (int)sw.ElapsedMilliseconds,
                    Status = "Failed",
                    ErrorMessage = string.IsNullOrEmpty(err)
                        ? "Could not parse Anthropic response body."
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
            logger.LogError(ex, "Anthropic request threw");
            return AiExecutionFailures.FromException(ex, (int)sw.ElapsedMilliseconds);
        }
    }
}
