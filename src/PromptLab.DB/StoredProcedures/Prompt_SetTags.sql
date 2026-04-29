CREATE PROCEDURE [dbo].[Prompt_SetTags]
    @PromptId UNIQUEIDENTIFIER,
    @TagIds NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Id] = @PromptId)
    BEGIN
        SELECT CAST(0 AS BIT) AS [Success], @PromptId AS [EntityId], N'Prompt not found.' AS [Message];
        RETURN;
    END

    DECLARE @ParsedTags TABLE ([TagId] UNIQUEIDENTIFIER PRIMARY KEY);

    IF @TagIds IS NOT NULL AND LEN(LTRIM(RTRIM(@TagIds))) > 0
    BEGIN
        INSERT INTO @ParsedTags ([TagId])
        SELECT DISTINCT TRY_CAST([value] AS UNIQUEIDENTIFIER)
        FROM STRING_SPLIT(@TagIds, ',')
        WHERE TRY_CAST([value] AS UNIQUEIDENTIFIER) IS NOT NULL;
    END

    DELETE pt
    FROM [dbo].[PromptTags] pt
    WHERE pt.[PromptId] = @PromptId
      AND NOT EXISTS (SELECT 1 FROM @ParsedTags t WHERE t.[TagId] = pt.[TagId]);

    INSERT INTO [dbo].[PromptTags] ([PromptId], [TagId], [CreatedAt])
    SELECT @PromptId, t.[TagId], SYSUTCDATETIME()
    FROM @ParsedTags t
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[PromptTags] pt
        WHERE pt.[PromptId] = @PromptId
          AND pt.[TagId] = t.[TagId]
    );

    SELECT CAST(1 AS BIT) AS [Success], @PromptId AS [EntityId], CAST(NULL AS NVARCHAR(500)) AS [Message];
END;
