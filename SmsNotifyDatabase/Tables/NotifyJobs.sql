CREATE TABLE [dbo].[NotifyJobs]
(
	[Id] BIGINT IDENTITY(1,1) PRIMARY KEY, 
    [UserId] BIGINT NOT NULL, 
    [JobName] NVARCHAR(50) NOT NULL, 
    [JobDescription] NVARCHAR(50) NULL,
    [Status] INT NOT NULL,  -- 0-NotStarted, 1-Running, 2-Completed, 3-Failed
    [JobCreateTime] DATETIME2 NULL,
    [JobLastUpdateTime] DATETIME2 NULL,

    CONSTRAINT [FK_NotifyJobs_ToTable] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]), 
)
