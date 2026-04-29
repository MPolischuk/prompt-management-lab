namespace PromptLab.Data.Infrastructure;

public static class StoredProcedures
{
    public const string PromptCreate = "dbo.Prompt_Create";
    public const string PromptUpdate = "dbo.Prompt_Update";
    public const string PromptDelete = "dbo.Prompt_Delete";
    public const string PromptGetById = "dbo.Prompt_GetById";
    public const string PromptSearch = "dbo.Prompt_Search";
    public const string PromptSetTags = "dbo.Prompt_SetTags";
    public const string TagCreate = "dbo.Tag_Create";
    public const string TagGetAll = "dbo.Tag_GetAll";
    public const string TagSearch = "dbo.Tag_Search";
    public const string AnalyzeCreateRun = "dbo.Analyze_CreateRun";
    public const string AnalyzeGetRunById = "dbo.Analyze_GetRunById";
}
