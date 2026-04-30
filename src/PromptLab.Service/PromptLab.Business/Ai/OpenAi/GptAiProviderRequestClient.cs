using System.Net.Http.Headers;
using System.Text;
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
        var connection = options.CurrentValue.Providers.OpenAi;
        if (string.IsNullOrWhiteSpace(connection.ApiKey))
        {
            return AiExecutionFailures.MissingApiKey("openai");
        }

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
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", connection.ApiKey.Trim());

        return await AiHttpExecutionPipeline.ExecuteAsync(
            client,
            httpRequest,
            JsonResponseExtractors.TryGetOpenAiText,
            logger,
            "OpenAI",
            cancellationToken);
    }
}
