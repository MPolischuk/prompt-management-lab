CREATE PROCEDURE [dbo].[Tag_Create]
    @Name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Slug NVARCHAR(120) = [dbo].[fn_NormalizeSlug](@Name);
    DECLARE @Id UNIQUEIDENTIFIER = NEWID();

    IF EXISTS (SELECT 1 FROM [dbo].[Tags] WHERE [Slug] = @Slug)
    BEGIN
        SELECT CAST(0 AS BIT) AS [Success], CAST(NULL AS UNIQUEIDENTIFIER) AS [EntityId], N'Tag already exists.' AS [Message];
        RETURN;
    END

    INSERT INTO [dbo].[Tags] ([Id], [Name], [Slug], [CreatedAt], [UpdatedAt])
    VALUES (@Id, @Name, @Slug, SYSUTCDATETIME(), SYSUTCDATETIME());

    SELECT CAST(1 AS BIT) AS [Success], @Id AS [EntityId], CAST(NULL AS NVARCHAR(500)) AS [Message];
END;
