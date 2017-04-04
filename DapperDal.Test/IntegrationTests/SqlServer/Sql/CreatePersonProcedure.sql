declare @sqlCmd nvarchar = '
IF (OBJECT_ID(''P_GetPersonsByCarId'') IS NOT NULL)
BEGIN
    DROP PROCEDURE dbo.P_GetPersonsByCarId
END

CREATE PROCEDURE dbo.P_GetPersonsByCarId (@CarId INT)
AS
    BEGIN
        SELECT  *
        FROM    Person
        WHERE   CarId = @CarId;
    END;

IF (OBJECT_ID(''P_GetPersonModelsByCarId'') IS NOT NULL)
BEGIN
    DROP PROCEDURE dbo.P_GetPersonModelsByCarId
END

CREATE PROCEDURE dbo.P_GetPersonModelsByCarId (@CarId INT)
AS
    BEGIN
        SELECT  PersonName AS [Name]
               ,CarId
        FROM    Person
        WHERE   CarId = @CarId;
    END;

IF (OBJECT_ID(''P_GetPersonMultipleModelsByCarId'') IS NOT NULL)
BEGIN
    DROP PROCEDURE dbo.P_GetPersonMultipleModelsByCarId
END

CREATE PROCEDURE dbo.P_GetPersonMultipleModelsByCarId (@CarId INT)
AS
    BEGIN
        SELECT  *
        FROM    Person
        WHERE   CarId = @CarId;

        SELECT  PersonName AS [Name]
               ,CarId
        FROM    Person
        WHERE   CarId = @CarId;
    END;

IF (OBJECT_ID(''P_GetPersonModelsByCarId_OutputCount'') IS NOT NULL)
BEGIN
    DROP PROCEDURE dbo.P_GetPersonModelsByCarId_OutputCount
END

CREATE PROCEDURE dbo.P_GetPersonModelsByCarId_OutputCount (@CarId INT, @TotalCount INT Output)
AS
    BEGIN
        SELECT @TotalCount = COUNT(1)
        FROM    Person
        WHERE   CarId = @CarId;

        SELECT  PersonName AS [Name]
               ,CarId
        FROM    Person
        WHERE   CarId = @CarId;
    END;
'

EXECUTE (N'USE [dapperTest]; EXEC sp_executesql N'''+@sqlCmd+'''')
