CREATE TABLE [dbo].[ScheduledJobs]
(
	[Id] BIGINT IDENTITY(1,1)  PRIMARY KEY, 
    [JobType] INT NOT NULL,  -- 1-OneTimeJob / 2-DailyJob / 3-WeeklyJob / 4-MonthlyJob / 5-YearlyJob
    [TimeArray] NVARCHAR(500) NULL, 
    [DaysOfWeek] INT NULL,   -- 1-Sunday / 2-Monday / 4-Tuesday / 8-Wednesday / 16-Thursday / 32-Friday / 64-Saturday / 127-AllDays
    [WeeksOfMonth] INT NULL, -- 1-week1 / 2-week2 / 4-week3 / 8-week4 / 16-week5
    [MonthsOfYear] INT NULL, -- 1-Jan / 2-Feb / 4-March / .... /2^11-December / 2^12-1 == AllMonths
    [StartDatetimeUtc] DATETIME2 NULL, -- whether we start at a future time, optional
    [EndDatetimeUtc] DATETIME2 NULL,   -- whether to disable the job at some time
    [Enable] BIT NOT NULL,   -- 0-disable / 1-enable.
    [TimeZone] NVARCHAR(50) NOT NULL, -- e.g. China , Pacific time
    [NextJobDueTimeUtc] DATETIME2 NOT NULL -- when to trigger next job
)
