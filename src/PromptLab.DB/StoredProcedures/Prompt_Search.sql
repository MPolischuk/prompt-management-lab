CREATE PROCEDURE [dbo].[Prompt_Search]
    @Query NVARCHAR(200) = NULL,
    @Category NVARCHAR(100) = NULL,
    @Language NVARCHAR(20) = NULL,
    @IsActive BIT = NULL,
    @TagId UNIQUEIDENTIFIER = NULL,
    @CreatedFrom DATETIME2(3) = NULL,
    @CreatedTo DATETIME2(3) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 SET @PageSize = 20;
    IF @PageSize > 200 SET @PageSize = 200;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    ;WITH Filtered AS
    (
        SELECT
            p.[Id],
            p.[Title],
            p.[Description],
            p.[Category],
            p.[Language],
            p.[ModelHint],
            p.[TargetModelId],
            p.[Temperature],
            p.[MaxTokens],
            p.[TopP],
            p.[IsActive],
            p.[CreatedAt],
            p.[UpdatedAt]
        FROM [dbo].[Prompts] p
        WHERE
            (@Query IS NULL OR p.[Title] LIKE N'%' + @Query + N'%' OR p.[Description] LIKE N'%' + @Query + N'%' OR p.[Content] LIKE N'%' + @Query + N'%')
            AND (@Category IS NULL OR p.[Category] = @Category)
            AND (@Language IS NULL OR p.[Language] = @Language)
            AND (@IsActive IS NULL OR p.[IsActive] = @IsActive)
            AND (@CreatedFrom IS NULL OR p.[CreatedAt] >= @CreatedFrom)
            AND (@CreatedTo IS NULL OR p.[CreatedAt] <= @CreatedTo)
            AND
            (
                @TagId IS NULL
                OR EXISTS
                (
                    SELECT 1
                    FROM [dbo].[PromptTags] pt
                    WHERE pt.[PromptId] = p.[Id]
                      AND pt.[TagId] = @TagId
                )
            )
    )
    SELECT
        f.*,
        COUNT(1) OVER() AS [TotalRows]
    FROM Filtered f
    ORDER BY f.[UpdatedAt] DESC
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
