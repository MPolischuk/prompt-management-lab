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
        p.[Temperature],
        p.[MaxTokens],
        p.[IsActive],
        p.[CreatedAt],
        p.[UpdatedAt]
    FROM [dbo].[Prompts] p
    WHERE p.[Id] = @Id;
END;
