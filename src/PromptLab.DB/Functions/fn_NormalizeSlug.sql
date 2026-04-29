CREATE FUNCTION [dbo].[fn_NormalizeSlug]
(
    @input NVARCHAR(200)
)
RETURNS NVARCHAR(200)
AS
BEGIN
    DECLARE @value NVARCHAR(200) = LOWER(LTRIM(RTRIM(ISNULL(@input, N''))));

    SET @value = REPLACE(@value, N' ', N'-');
    SET @value = REPLACE(@value, N'--', N'-');
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

    RETURN @value;
END;
