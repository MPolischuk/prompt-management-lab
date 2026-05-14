/*
  Cuatro ejecuciones de test por suite seed (modelo + temperatura + estado unicos por suite).
*/

DECLARE @SuiteId UNIQUEIDENTIFIER;
DECLARE @PromptId UNIQUEIDENTIFIER;
DECLARE @PromptVersion INT;

DECLARE run_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT
    ts.[Id],
    ts.[PromptId],
    p.[Version]
FROM [dbo].[TestSuites] AS ts
INNER JOIN [dbo].[Prompts] AS p
    ON p.[Id] = ts.[PromptId]
WHERE
    ts.[Name] LIKE N'Suite seed v1:%'
    AND ts.[IsActive] = 1;

OPEN run_cursor;

FETCH NEXT FROM run_cursor
INTO @SuiteId, @PromptId, @PromptVersion;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestRuns]
        WHERE [SuiteId] = @SuiteId
          AND [Model] = N'gpt-4o'
          AND [Temperature] = 0.21
          AND [Status] = N'completed'
    )
    BEGIN
        EXEC [dbo].[TestRun_Create]
            @SuiteId = @SuiteId,
            @PromptId = @PromptId,
            @PromptVersion = @PromptVersion,
            @Model = N'gpt-4o',
            @Temperature = 0.21,
            @Status = N'completed';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestRuns]
        WHERE [SuiteId] = @SuiteId
          AND [Model] = N'gpt-4o-mini'
          AND [Temperature] = 0.52
          AND [Status] = N'failed'
    )
    BEGIN
        EXEC [dbo].[TestRun_Create]
            @SuiteId = @SuiteId,
            @PromptId = @PromptId,
            @PromptVersion = @PromptVersion,
            @Model = N'gpt-4o-mini',
            @Temperature = 0.52,
            @Status = N'failed';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestRuns]
        WHERE [SuiteId] = @SuiteId
          AND [Model] = N'claude-3-5-sonnet'
          AND [Temperature] = 0.73
          AND [Status] = N'pending'
    )
    BEGIN
        EXEC [dbo].[TestRun_Create]
            @SuiteId = @SuiteId,
            @PromptId = @PromptId,
            @PromptVersion = @PromptVersion,
            @Model = N'claude-3-5-sonnet',
            @Temperature = 0.73,
            @Status = N'pending';
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[TestRuns]
        WHERE [SuiteId] = @SuiteId
          AND [Model] = N'gemini-1.5-pro'
          AND [Temperature] = 0.94
          AND [Status] = N'running'
    )
    BEGIN
        EXEC [dbo].[TestRun_Create]
            @SuiteId = @SuiteId,
            @PromptId = @PromptId,
            @PromptVersion = @PromptVersion,
            @Model = N'gemini-1.5-pro',
            @Temperature = 0.94,
            @Status = N'running';
    END;

    FETCH NEXT FROM run_cursor
    INTO @SuiteId, @PromptId, @PromptVersion;
END;

CLOSE run_cursor;
DEALLOCATE run_cursor;
