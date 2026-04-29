CREATE FUNCTION [dbo].[fn_HasPromptTag]
(
    @promptId UNIQUEIDENTIFIER,
    @tagId UNIQUEIDENTIFIER
)
RETURNS BIT
AS
BEGIN
    DECLARE @result BIT = 0;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[PromptTags] pt
        WHERE pt.[PromptId] = @promptId
          AND pt.[TagId] = @tagId
    )
    BEGIN
        SET @result = 1;
    END

    RETURN @result;
END;
