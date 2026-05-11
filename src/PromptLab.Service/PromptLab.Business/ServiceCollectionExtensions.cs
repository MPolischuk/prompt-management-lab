using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PromptLab.Business.Ai;
using PromptLab.Business.Configuration;
using PromptLab.Business.Services;
using PromptLab.Business.Services.Contracts;

namespace PromptLab.Business;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPromptLabBusiness(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));
        services.AddSingleton<IValidateOptions<AiOptions>, AiOptionsValidator>();
        services.AddOptions<AiOptions>()
            .Bind(configuration.GetSection(AiOptions.SectionName))
            .ValidateOnStart();

        services.AddMemoryCache();

        services.AddScoped<AiProviderFactory>();
        services.AddScoped<IPromptService, PromptService>();
        services.AddScoped<ITestSuiteService, TestSuiteService>();
        services.AddScoped<ITestCaseService, TestCaseService>();
        services.AddScoped<ITestRunService, TestRunService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IAnalyzeService, AnalyzeService>();
        return services;
    }
}
