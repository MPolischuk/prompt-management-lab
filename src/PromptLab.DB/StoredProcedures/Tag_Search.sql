CREATE PROCEDURE [dbo].[Tag_Search]
    @Query NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.[Id],
        t.[Name],
        t.[Slug],
        t.[CreatedAt],
        t.[UpdatedAt]
    FROM [dbo].[Tags] t
    WHERE @Query IS NULL
       OR t.[Name] LIKE N'%' + @Query + N'%'
       OR t.[Slug] LIKE N'%' + @Query + N'%'
    ORDER BY t.[Name] ASC;
END;
