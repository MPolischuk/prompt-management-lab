using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromptLab.Business.Ai;
using PromptLab.Business.Configuration;
using PromptLab.Business.Services;

namespace PromptLab.Business;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPromptLabBusiness(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));
        services.Configure<AiOptions>(configuration.GetSection(AiOptions.SectionName));

        services.AddMemoryCache();
        services.AddScoped<IAiProvider, SimulatedAiProvider>();
        services.AddScoped<AiProviderFactory>();
        services.AddScoped<IPromptService, PromptService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IAnalyzeService, AnalyzeService>();
        return services;
    }
}
