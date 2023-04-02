CREATE PROCEDURE [dbo].[UpdateScheduledJobNextJobDueTime_V0]
	@ScheduledJobId BIGINT,
	@NextJobDueTimeUtc DATETIME2
AS

	UPDATE [dbo].[ScheduledJobs]
		SET [NextJobDueTimeUtc] = @NextJobDueTimeUtc
		WHERE [Id] <= @ScheduledJobId
RETURN 0
