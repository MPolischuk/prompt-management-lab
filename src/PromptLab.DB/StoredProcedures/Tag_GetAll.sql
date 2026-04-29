CREATE PROCEDURE [dbo].[Tag_GetAll]
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
    ORDER BY t.[Name] ASC;
END;
