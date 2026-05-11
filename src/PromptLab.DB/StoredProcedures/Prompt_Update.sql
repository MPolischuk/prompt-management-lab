CREATE PROCEDURE [dbo].[Prompt_Update]
    @Id UNIQUEIDENTIFIER,
    @Title NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @Content NVARCHAR(MAX),
    @Category NVARCHAR(100) = NULL,
    @Language NVARCHAR(20) = NULL,
    @ModelHint NVARCHAR(100) = NULL,
    @TargetModelId NVARCHAR(100) = NULL,
    @Temperature DECIMAL(4, 2) = NULL,
    @MaxTokens INT = NULL,
    @TopP DECIMAL(4, 2) = NULL,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Title] = @Title AND [Id] <> @Id)
    BEGIN
        SELECT CAST(0 AS BIT) AS [Success], @Id AS [EntityId], N'Prompt title already exists.' AS [Message];
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Id] = @Id)
    BEGIN
        SELECT CAST(0 AS BIT) AS [Success], @Id AS [EntityId], N'Prompt not found.' AS [Message];
        RETURN;
    END

    DECLARE @OldContent NVARCHAR(MAX);
    DECLARE @OldVersion INT;

    SELECT
        @OldContent = [Content],
        @OldVersion = [Version]
    FROM [dbo].[Prompts]
    WHERE [Id] = @Id;

    DECLARE @NewVersion INT =
        CASE
            WHEN @Content <> @OldContent THEN @OldVersion + 1
            ELSE @OldVersion
        END;

    UPDATE [dbo].[Prompts]
       SET [Title] = @Title,
           [Description] = @Description,
           [Content] = @Content,
           [Category] = @Category,
           [Language] = @Language,
           [ModelHint] = @ModelHint,
           [TargetModelId] = @TargetModelId,
           [Temperature] = @Temperature,
           [MaxTokens] = @MaxTokens,
           [TopP] = @TopP,
           [IsActive] = @IsActive,
           [Version] = @NewVersion,
           [UpdatedAt] = SYSUTCDATETIME()
     WHERE [Id] = @Id;

    IF @Content <> @OldContent
    BEGIN
        INSERT INTO [dbo].[PromptVersions] ([Id], [PromptId], [Content], [Version], [CreatedAt])
        VALUES (NEWID(), @Id, @Content, @NewVersion, SYSUTCDATETIME());
    END

    SELECT
        CAST(1 AS BIT) AS [Success],
        @Id AS [EntityId],
        CAST(NULL AS NVARCHAR(500)) AS [Message];
END;
