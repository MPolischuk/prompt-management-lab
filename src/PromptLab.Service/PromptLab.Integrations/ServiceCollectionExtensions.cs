using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using PromptLab.Business.Ai;
using PromptLab.Business.Ai.Anthropic;
using PromptLab.Business.Ai.Contracts;
using PromptLab.Business.Ai.Google;
using PromptLab.Business.Ai.OpenAi;
using PromptLab.Business.Configuration;

namespace PromptLab.Integrations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPromptLabIntegrations(this IServiceCollection services, IConfiguration configuration)
    {
        var aiSnapshot = configuration.GetSection(AiOptions.SectionName).Get<AiOptions>() ?? new AiOptions();
        RegisterVendorProviders(services, aiSnapshot);
        return services;
    }

    private static void RegisterVendorProviders(IServiceCollection services, AiOptions aiSnapshot)
    {
        var conn = aiSnapshot.Providers;

        if (conn.OpenAi.Enabled)
        {
            services.AddHttpClient(AiHttpClientNames.OpenAi, (sp, client) =>
                ConfigureHttpClientBase(
                    client,
                    GetConnection(sp).OpenAi.BaseUrl,
                    "https://api.openai.com",
                    GetConnection(sp).OpenAi.TimeoutSeconds));
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IAiProvider, GptAiProvider>());
            if (conn.OpenAi.Mock)
            {
                services.AddScoped<IGptAiProviderRequestClient, GptAiProviderRequestMock>();
            }
            else
            {
                services.AddScoped<IGptAiProviderRequestClient, GptAiProviderRequestClient>();
            }
        }

        if (conn.Anthropic.Enabled)
        {
            services.AddHttpClient(AiHttpClientNames.Anthropic, (sp, client) =>
                ConfigureHttpClientBase(
                    client,
                    GetConnection(sp).Anthropic.BaseUrl,
                    "https://api.anthropic.com",
                    GetConnection(sp).Anthropic.TimeoutSeconds));
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IAiProvider, ClaudeAiProvider>());
            if (conn.Anthropic.Mock)
            {
                services.AddScoped<IClaudeAiProviderRequestClient, ClaudeAiProviderRequestMock>();
            }
            else
            {
                services.AddScoped<IClaudeAiProviderRequestClient, ClaudeAiProviderRequestClient>();
            }
        }

        if (conn.Google.Enabled)
        {
            services.AddHttpClient(AiHttpClientNames.Google, (sp, client) =>
                ConfigureHttpClientBase(
                    client,
                    GetConnection(sp).Google.BaseUrl,
                    "https://generativelanguage.googleapis.com",
                    GetConnection(sp).Google.TimeoutSeconds));
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IAiProvider, GeminiAiProvider>());
            if (conn.Google.Mock)
            {
                services.AddScoped<IGeminiAiProviderRequestClient, GeminiAiProviderRequestMock>();
            }
            else
            {
                services.AddScoped<IGeminiAiProviderRequestClient, GeminiAiProviderRequestClient>();
            }
        }

        return;

        static AiProvidersConnectionOptions GetConnection(IServiceProvider sp) =>
            sp.GetRequiredService<IOptions<AiOptions>>().Value.Providers;

        static void ConfigureHttpClientBase(HttpClient client, string? baseUrl, string fallbackBaseUrl, int timeoutSeconds)
        {
            var resolved = string.IsNullOrWhiteSpace(baseUrl) ? fallbackBaseUrl : baseUrl.Trim().TrimEnd('/');
            client.BaseAddress = new Uri(resolved + "/");
            var seconds = timeoutSeconds > 0 ? timeoutSeconds : 120;
            client.Timeout = TimeSpan.FromSeconds(seconds);
        }
    }
}
