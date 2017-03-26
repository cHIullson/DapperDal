declare @sqlCmd nvarchar = '
IF (OBJECT_ID(''P_GetPersonsByCarId'') IS NOT NULL)
BEGIN
	DROP PROCEDURE P_GetPersonsByCarId
END

CREATE PROCEDURE dbo.P_GetPersonsByCarId (@CarId int)
AS
BEGIN
	select * from Person where CarId = @CarId
END

IF (OBJECT_ID(''P_GetPersonModelsByCarId'') IS NOT NULL)
BEGIN
	DROP PROCEDURE P_GetPersonModelsByCarId
END

CREATE PROCEDURE P_GetPersonModelsByCarId
	@CarId int
AS
BEGIN
	select PersonName as [Name], CarId from Person where CarId = @CarId
END
'

EXECUTE (N'USE [dapperTest]; EXEC sp_executesql N'''+@sqlCmd+'''')