CREATE PROCEDURE [dbo].[GetScheduledJobsToRunNow_V0]
AS

    DECLARE @CurrentTimeUtc             DATETIME2(7) = GETUTCDATE()

    SELECT * FROM [dbo].[ScheduledJobs]
        WHERE [NextJobDueTimeUtc] <= @CurrentTimeUtc
RETURN 0
