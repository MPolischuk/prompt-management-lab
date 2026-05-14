/*
  Cuatro corridas de analisis por prompt activo (proveedores y latencias variadas, idempotente).
*/

DECLARE @PromptId UNIQUEIDENTIFIER;
DECLARE @Snapshot NVARCHAR(MAX);
DECLARE @Temperature DECIMAL(4, 2);
DECLARE @MaxTokens INT;
DECLARE @TopP DECIMAL(4, 2);

DECLARE ar_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT
    [Id],
    [Content],
    [Temperature],
    [MaxTokens],
    [TopP]
FROM [dbo].[Prompts]
WHERE [IsActive] = 1;

OPEN ar_cursor;

FETCH NEXT FROM ar_cursor
INTO @PromptId, @Snapshot, @Temperature, @MaxTokens, @TopP;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[AnalysisRuns]
        WHERE [PromptId] = @PromptId
          AND [Provider] = N'openai'
          AND [ModelId] = N'gpt-4o'
          AND [LatencyMs] = 237
    )
    BEGIN
        EXEC [dbo].[Analyze_CreateRun]
            @PromptId = @PromptId,
            @Provider = N'openai',
            @ModelId = N'gpt-4o',
            @Input = N'[SEED] input openai caso 1',
            @Output = N'[SEED] output simulado openai gpt-4o',
            @Temperature = @Temperature,
            @MaxTokens = @MaxTokens,
            @TopP = @TopP,
            @PromptSnapshot = @Snapshot,
            @PromptSnapshotHash = N'SEED-OPENAI-GPT4O-237',
            @Status = N'Completed',
            @ErrorMessage = NULL,
            @LatencyMs = 237;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[AnalysisRuns]
        WHERE [PromptId] = @PromptId
          AND [Provider] = N'anthropic'
          AND [ModelId] = N'claude-3-5-sonnet'
          AND [LatencyMs] = 890
    )
    BEGIN
        EXEC [dbo].[Analyze_CreateRun]
            @PromptId = @PromptId,
            @Provider = N'anthropic',
            @ModelId = N'claude-3-5-sonnet',
            @Input = N'[SEED] input anthropic caso 2',
            @Output = NULL,
            @Temperature = @Temperature,
            @MaxTokens = @MaxTokens,
            @TopP = @TopP,
            @PromptSnapshot = @Snapshot,
            @PromptSnapshotHash = N'SEED-ANTHROPIC-890',
            @Status = N'Failed',
            @ErrorMessage = N'[SEED] Error simulado: rate limit',
            @LatencyMs = 890;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[AnalysisRuns]
        WHERE [PromptId] = @PromptId
          AND [Provider] = N'google'
          AND [ModelId] = N'gemini-1.5-pro'
          AND [LatencyMs] = 1540
    )
    BEGIN
        EXEC [dbo].[Analyze_CreateRun]
            @PromptId = @PromptId,
            @Provider = N'google',
            @ModelId = N'gemini-1.5-pro',
            @Input = N'[SEED] input google caso 3',
            @Output = N'[SEED] output simulado gemini',
            @Temperature = @Temperature,
            @MaxTokens = @MaxTokens,
            @TopP = @TopP,
            @PromptSnapshot = @Snapshot,
            @PromptSnapshotHash = N'SEED-GOOGLE-1540',
            @Status = N'Completed',
            @ErrorMessage = NULL,
            @LatencyMs = 1540;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[AnalysisRuns]
        WHERE [PromptId] = @PromptId
          AND [Provider] = N'simulated'
          AND [ModelId] = N'simulated-perf'
          AND [LatencyMs] = 2890
    )
    BEGIN
        EXEC [dbo].[Analyze_CreateRun]
            @PromptId = @PromptId,
            @Provider = N'simulated',
            @ModelId = N'simulated-perf',
            @Input = N'[SEED] input simulated caso 4',
            @Output = NULL,
            @Temperature = @Temperature,
            @MaxTokens = @MaxTokens,
            @TopP = @TopP,
            @PromptSnapshot = @Snapshot,
            @PromptSnapshotHash = N'SEED-SIM-PERF-2890',
            @Status = N'Running',
            @ErrorMessage = NULL,
            @LatencyMs = 2890;
    END;

    FETCH NEXT FROM ar_cursor
    INTO @PromptId, @Snapshot, @Temperature, @MaxTokens, @TopP;
END;

CLOSE ar_cursor;
DEALLOCATE ar_cursor;
