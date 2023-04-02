CREATE TABLE [dbo].[Users]
(
	[Id] BIGINT IDENTITY(1,1) PRIMARY KEY, 
    [FirstName] NVARCHAR(50) NOT NULL, 
    [LastName] NVARCHAR(50) NOT NULL, 
    [Email] NVARCHAR(50) NULL, 
    [CellphoneNumber] VARCHAR(50) NOT NULL
)
