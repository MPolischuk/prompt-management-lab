CREATE FUNCTION [dbo].[fn_NormalizeSlug]
(
    @input NVARCHAR(200)
)
RETURNS NVARCHAR(200)
AS
BEGIN
    DECLARE @inputNorm NVARCHAR(200);
    DECLARE @value NVARCHAR(200);

    SET @inputNorm = @input COLLATE DATABASE_DEFAULT;

    SET @value = LOWER(
                    LTRIM(
                        RTRIM(
                            ISNULL(@inputNorm, N'')
                        )
                    )
                 );

    SET @value = REPLACE(@value, N' ', N'-');
    SET @value = REPLACE(@value, N'/', N'-');
    SET @value = REPLACE(@value, N'\', N'-');
    SET @value = REPLACE(@value, N'.', N'-');
    SET @value = REPLACE(@value, N',', N'-');

    WHILE CHARINDEX(N'--', @value) > 0
    BEGIN
        SET @value = REPLACE(@value, N'--', N'-');
    END

    IF LEFT(@value, 1) = N'-'
        SET @value = SUBSTRING(@value, 2, LEN(@value));

    IF RIGHT(@value, 1) = N'-'
        SET @value = LEFT(@value, LEN(@value) - 1);

    RETURN @value COLLATE DATABASE_DEFAULT;
END;