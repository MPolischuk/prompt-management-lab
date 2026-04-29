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
           [UpdatedAt] = SYSUTCDATETIME()
     WHERE [Id] = @Id;

    DECLARE @RowsAffected INT = @@ROWCOUNT;

    SELECT
        CASE WHEN @RowsAffected > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [Success],
        @Id AS [EntityId],
        CASE WHEN @RowsAffected > 0 THEN CAST(NULL AS NVARCHAR(500)) ELSE N'Prompt not found.' END AS [Message];
END;
