using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromptLab.Business.Repositories;
using PromptLab.Data.Configuration;
using PromptLab.Data.Infrastructure;
using PromptLab.Data.Repositories;
using RepoDb;

namespace PromptLab.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPromptLabData(this IServiceCollection services, IConfiguration configuration)
    {
        GlobalConfiguration.Setup().UseSqlServer();

        services.Configure<SqlOptions>(configuration.GetSection(SqlOptions.SectionName));
        services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IPromptRepository, PromptRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IAnalyzeRepository, AnalyzeRepository>();
        return services;
    }
}
