using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PromptLab.Business.Configuration;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai.Google;

/// <summary>
/// Cliente HTTP para Gemini generateContent (<c>v1beta/models/{model}:generateContent</c>).
/// </summary>
public class GeminiAiProviderRequestClient(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<AiOptions> options,
    ILogger<GeminiAiProviderRequestClient> logger) : IGeminiAiProviderRequestClient
{
    public async Task<AnalyzeExecutionResult> AnalyzeAsync(
        Prompt prompt,
        AnalyzeExecutionRequest request,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var connection = options.CurrentValue.Providers.Google;
            if (string.IsNullOrWhiteSpace(connection.ApiKey))
                return AiExecutionFailures.MissingApiKey("google");

            var client = httpClientFactory.CreateClient(AiHttpClientNames.Google);
            var combined = AiPromptComposer.BuildUserFacingText(prompt, request);
            var settings = request.EffectiveSettings;

            var model = Uri.EscapeDataString(request.ModelId);
            var path = $"v1beta/models/{model}:generateContent?key={Uri.EscapeDataString(connection.ApiKey!.Trim())}";

            var parts = new JsonArray { new JsonObject { ["text"] = combined } };
            var contents = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "user",
                    ["parts"] = parts
                }
            };

            var generationConfig = new JsonObject();
            if (settings.Temperature.HasValue)
                generationConfig["temperature"] = (double)settings.Temperature.Value;
            if (settings.TopP.HasValue)
                generationConfig["topP"] = (double)settings.TopP.Value;
            if (settings.MaxTokens.HasValue)
                generationConfig["maxOutputTokens"] = settings.MaxTokens.Value;

            var body = new JsonObject { ["contents"] = contents };
            if (generationConfig.Count > 0)
                body["generationConfig"] = generationConfig;

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json")
            };

            using var response = await client.SendAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            sw.Stop();

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Gemini request failed: {Status} {Body}", (int)response.StatusCode, raw);
                return AiExecutionFailures.FromHttpError((int)response.StatusCode, raw, (int)sw.ElapsedMilliseconds);
            }

            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;
            var text = JsonResponseExtractors.TryGetGeminiText(root);
            if (string.IsNullOrEmpty(text))
            {
                var err = JsonResponseExtractors.TryGetErrorMessage(root);
                return new AnalyzeExecutionResult
                {
                    Output = string.Empty,
                    LatencyMs = (int)sw.ElapsedMilliseconds,
                    Status = "Failed",
                    ErrorMessage = string.IsNullOrEmpty(err)
                        ? "Could not parse Gemini response body."
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
            logger.LogError(ex, "Gemini request threw");
            return AiExecutionFailures.FromException(ex, (int)sw.ElapsedMilliseconds);
        }
    }
}
