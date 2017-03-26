IF (OBJECT_ID('Person') IS NOT NULL)
BEGIN
    DROP TABLE Person
END

CREATE TABLE Person (
    PersonId INT IDENTITY(1,1) PRIMARY KEY,
    PersonName NVARCHAR(50) NULL,
    CarId INT NULL,
    [CreateTime] [DATETIME] NULL DEFAULT (GETDATE()),
    [UpdateTime] [DATETIME] NULL,
    [IsActive] [BIT] NULL DEFAULT ((0))
)