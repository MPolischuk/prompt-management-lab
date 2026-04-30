using Microsoft.Extensions.Options;
using PromptLab.Business.Ai;
using PromptLab.Business.Ai.Contracts;
using PromptLab.Business.Configuration;
using PromptLab.Entities.Analyze;
using PromptLab.Entities.Common;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Services.Analyze;

internal interface IAnalyzeModelResolver
{
    OperationResult? TryResolve(AnalyzeRequest request, Prompt prompt, out AnalyzeResolution? resolution);
}

internal sealed class AnalyzeModelResolver(
    AiProviderFactory providerFactory,
    IOptions<AiOptions> aiOptions) : IAnalyzeModelResolver
{
    public OperationResult? TryResolve(AnalyzeRequest request, Prompt prompt, out AnalyzeResolution? resolution)
    {
        var models = AnalyzeModelCatalog.GetConfiguredModels(aiOptions.Value);
        var modelCatalog = models
            .Where(static m => m.Enabled)
            .ToDictionary(m => m.Id, StringComparer.OrdinalIgnoreCase);

        var selectedModel = ResolveModel(request, prompt, modelCatalog);
        if (selectedModel is null)
        {
            resolution = null;
            return new OperationResult { Success = false, Message = "Model not configured.", ErrorCode = OperationErrorCode.Validation };
        }

        var selectedProvider = ResolveProvider(request, selectedModel);
        var provider = providerFactory.Resolve(selectedProvider);
        if (provider is null)
        {
            resolution = null;
            return new OperationResult { Success = false, Message = "Provider not configured.", ErrorCode = OperationErrorCode.Unavailable };
        }

        if (!provider.Name.Equals(selectedModel.Provider, StringComparison.OrdinalIgnoreCase))
        {
            resolution = null;
            return new OperationResult
            {
                Success = false,
                Message = "Model does not belong to selected provider.",
                ErrorCode = OperationErrorCode.Validation
            };
        }

        resolution = new AnalyzeResolution(selectedModel, provider);
        return null;
    }

    private static string? ResolveProvider(AnalyzeRequest request, AiModelOption selectedModel)
    {
        return string.IsNullOrWhiteSpace(request.Provider) ? selectedModel.Provider : request.Provider;
    }

    private static AiModelOption? ResolveModel(
        AnalyzeRequest request,
        Prompt prompt,
        IReadOnlyDictionary<string, AiModelOption> modelCatalog)
    {
        if (!string.IsNullOrWhiteSpace(request.ModelId))
        {
            return modelCatalog.GetValueOrDefault(request.ModelId);
        }

        if (!string.IsNullOrWhiteSpace(prompt.TargetModelId))
        {
            return modelCatalog.GetValueOrDefault(prompt.TargetModelId);
        }

        if (!string.IsNullOrWhiteSpace(prompt.ModelHint))
        {
            return modelCatalog.GetValueOrDefault(prompt.ModelHint);
        }

        return modelCatalog.Values.FirstOrDefault();
    }
}

internal sealed record AnalyzeResolution(
    AiModelOption Model,
    IAiProvider Provider);
