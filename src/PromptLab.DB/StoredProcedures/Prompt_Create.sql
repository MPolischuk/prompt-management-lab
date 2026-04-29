CREATE PROCEDURE [dbo].[Prompt_Create]
    @Title NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @Content NVARCHAR(MAX),
    @Category NVARCHAR(100) = NULL,
    @Language NVARCHAR(20) = NULL,
    @ModelHint NVARCHAR(100) = NULL,
    @TargetModelId NVARCHAR(100) = NULL,
    @Temperature DECIMAL(4, 2) = NULL,
    @MaxTokens INT = NULL,
    @TopP DECIMAL(4, 2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @UtcNow DATETIME2(3) = SYSUTCDATETIME();

    IF EXISTS (SELECT 1 FROM [dbo].[Prompts] WHERE [Title] = @Title)
    BEGIN
        SELECT CAST(0 AS BIT) AS [Success], CAST(NULL AS UNIQUEIDENTIFIER) AS [EntityId], N'Prompt title already exists.' AS [Message];
        RETURN;
    END

    INSERT INTO [dbo].[Prompts]
    (
        [Id], [Title], [Description], [Content], [Category], [Language],
        [ModelHint], [TargetModelId], [Temperature], [MaxTokens], [TopP], [IsActive], [CreatedAt], [UpdatedAt]
    )
    VALUES
    (
        @Id, @Title, @Description, @Content, @Category, @Language,
        @ModelHint, @TargetModelId, @Temperature, @MaxTokens, @TopP, 1, @UtcNow, @UtcNow
    );

    SELECT
        CAST(1 AS BIT) AS [Success],
        @Id AS [EntityId],
        CAST(NULL AS NVARCHAR(500)) AS [Message];
END;
