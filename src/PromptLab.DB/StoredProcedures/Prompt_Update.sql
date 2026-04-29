CREATE PROCEDURE [dbo].[Prompt_Update]
    @Id UNIQUEIDENTIFIER,
    @Title NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @Content NVARCHAR(MAX),
    @Category NVARCHAR(100) = NULL,
    @Language NVARCHAR(20) = NULL,
    @ModelHint NVARCHAR(100) = NULL,
    @Temperature DECIMAL(4, 2) = NULL,
    @MaxTokens INT = NULL,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[Prompts]
       SET [Title] = @Title,
           [Description] = @Description,
           [Content] = @Content,
           [Category] = @Category,
           [Language] = @Language,
           [ModelHint] = @ModelHint,
           [Temperature] = @Temperature,
           [MaxTokens] = @MaxTokens,
           [IsActive] = @IsActive,
           [UpdatedAt] = SYSUTCDATETIME()
     WHERE [Id] = @Id;

    SELECT
        CASE WHEN @@ROWCOUNT > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS [Success],
        @Id AS [EntityId],
        CASE WHEN @@ROWCOUNT > 0 THEN CAST(NULL AS NVARCHAR(500)) ELSE N'Prompt not found.' END AS [Message];
END;
