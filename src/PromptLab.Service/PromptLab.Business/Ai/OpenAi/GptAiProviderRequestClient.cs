using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PromptLab.Business.Configuration;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai.OpenAi;

/// <summary>
/// Cliente HTTP para OpenAI (endpoint <c>/v1/responses</c>).
/// </summary>
public class GptAiProviderRequestClient(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<AiOptions> options,
    ILogger<GptAiProviderRequestClient> logger) : IGptAiProviderRequestClient
{
    public async Task<AnalyzeExecutionResult> AnalyzeAsync(
        Prompt prompt,
        AnalyzeExecutionRequest request,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var connection = options.CurrentValue.Providers.OpenAi;
            if (string.IsNullOrWhiteSpace(connection.ApiKey))
                return AiExecutionFailures.MissingApiKey("openai");

            var client = httpClientFactory.CreateClient(AiHttpClientNames.OpenAi);
            var combined = AiPromptComposer.BuildUserFacingText(prompt, request);
            var settings = request.EffectiveSettings;

            var body = new JsonObject
            {
                ["model"] = request.ModelId,
                ["input"] = combined
            };

            if (settings.Temperature.HasValue)
                body["temperature"] = (double)settings.Temperature.Value;
            if (settings.TopP.HasValue)
                body["top_p"] = (double)settings.TopP.Value;
            if (settings.MaxTokens.HasValue)
                body["max_output_tokens"] = settings.MaxTokens.Value;

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/responses")
            {
                Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", connection.ApiKey!.Trim());

            using var response = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            sw.Stop();

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("OpenAI request failed: {Status} {Body}", (int)response.StatusCode, raw);
                return AiExecutionFailures.FromHttpError((int)response.StatusCode, raw, (int)sw.ElapsedMilliseconds);
            }

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;
            var text = JsonResponseExtractors.TryGetOpenAiText(root);
            if (string.IsNullOrEmpty(text))
            {
                var err = JsonResponseExtractors.TryGetErrorMessage(root);
                return new AnalyzeExecutionResult
                {
                    Output = string.Empty,
                    LatencyMs = (int)sw.ElapsedMilliseconds,
                    Status = "Failed",
                    ErrorMessage = string.IsNullOrEmpty(err)
                        ? "Could not parse OpenAI response body."
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
            logger.LogError(ex, "OpenAI request threw");
            return AiExecutionFailures.FromException(ex, (int)sw.ElapsedMilliseconds);
        }
    }
}
