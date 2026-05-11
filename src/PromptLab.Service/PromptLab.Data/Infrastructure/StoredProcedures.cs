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
    public const string PromptVersionGetByPromptId = "dbo.PromptVersion_GetByPromptId";
    public const string TestSuiteCreate = "dbo.TestSuite_Create";
    public const string TestSuiteUpdate = "dbo.TestSuite_Update";
    public const string TestSuiteDelete = "dbo.TestSuite_Delete";
    public const string TestSuiteGetById = "dbo.TestSuite_GetById";
    public const string TestSuiteGetByPromptId = "dbo.TestSuite_GetByPromptId";
    public const string TestCaseCreate = "dbo.TestCase_Create";
    public const string TestCaseUpdate = "dbo.TestCase_Update";
    public const string TestCaseDelete = "dbo.TestCase_Delete";
    public const string TestCaseGetBySuiteId = "dbo.TestCase_GetBySuiteId";
    public const string TestRunCreate = "dbo.TestRun_Create";
    public const string TestRunUpdate = "dbo.TestRun_Update";
    public const string TestRunGetById = "dbo.TestRun_GetById";
    public const string TestRunGetBySuiteId = "dbo.TestRun_GetBySuiteId";
    public const string TestRunGetAll = "dbo.TestRun_GetAll";
    public const string TestResultCreate = "dbo.TestResult_Create";
    public const string TestResultGetByRunId = "dbo.TestResult_GetByRunId";
}
