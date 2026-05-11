using PromptLab.Entities.Analyze;
using PromptLab.Entities.Prompts;

namespace PromptLab.Business.Ai;

internal static class AiPromptComposer
{
    /// <summary>
    /// Combina el contenido del prompt (instrucciones/plantilla) con el input opcional del usuario.
    /// </summary>
    internal static string BuildUserFacingText(Prompt prompt, AnalyzeExecutionRequest request)
    {
        var input = request.Input?.Trim();
        if (string.IsNullOrEmpty(input))
            return prompt.Content;

        return $"{prompt.Content}\n\n--- User input ---\n{input}";
    }
}
