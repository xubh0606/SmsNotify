CREATE PROCEDURE [dbo].[GetNotifyJobsToRunNow_V0]
AS

    DECLARE @CurrentTimeUtc       DATETIME2(7) = GETUTCDATE()

    SELECT * FROM [dbo].[NotifyJobs]
        WHERE [Status] = 0 -- NotStarted
RETURN 0
