CREATE PROCEDURE [dbo].[TestRun_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        r.[Id],
        r.[SuiteId],
        r.[PromptId],
        r.[PromptVersion],
        r.[Model],
        r.[Temperature],
        r.[Status],
        r.[StartedAt],
        r.[CompletedAt],
        r.[CreatedAt],
        p.[Title] AS [PromptTitle],
        s.[Name] AS [SuiteName]
    FROM [dbo].[TestRuns] r
    INNER JOIN [dbo].[Prompts] p ON p.[Id] = r.[PromptId]
    INNER JOIN [dbo].[TestSuites] s ON s.[Id] = r.[SuiteId]
    ORDER BY r.[CreatedAt] DESC;
END;
