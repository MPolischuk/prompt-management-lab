CREATE PROCEDURE [dbo].[Prompt_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.[Id],
        p.[Title],
        p.[Description],
        p.[Content],
        p.[Category],
        p.[Language],
        p.[ModelHint],
        p.[TargetModelId],
        p.[Temperature],
        p.[MaxTokens],
        p.[TopP],
        p.[Version],
        p.[IsActive],
        p.[CreatedAt],
        p.[UpdatedAt],
        (
            SELECT
                t.[Id],
                t.[Name],
                t.[Slug]
            FROM [dbo].[PromptTags] pt
            INNER JOIN [dbo].[Tags] t ON t.[Id] = pt.[TagId]
            WHERE pt.[PromptId] = p.[Id]
            FOR JSON PATH
        ) AS [TagsJson]
    FROM [dbo].[Prompts] p
    WHERE p.[Id] = @Id;
END;
