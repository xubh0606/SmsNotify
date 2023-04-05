CREATE PROCEDURE [dbo].[CreateNotifyJob_V0]
    @ScheduledJobId BIGINT
AS

    DECLARE    @JobName         NVARCHAR(50);
    DECLARE    @JobDescription  NVARCHAR(50);
    DECLARE    @UtcDateNow      DATETIME2;
    DECLARE    @UserId          BIGINT;
    DECLARE    @NotifyJobId     BIGINT;

    SET @UtcDateNow = SYSUTCDATETIME()

    SELECT @JobName = [JobName], @JobDescription = [JobDescription], @UserId = [UserId]
        FROM [ScheduledJobs]
        WHERE [Id] = @ScheduledJobId

    IF (@UserId IS NULL)
        BEGIN
            DECLARE @ErrorMessage NVARCHAR(100) = concat('Scheduled job not found, ScheduledJobId: ', @ScheduledJobId);
            THROW 60404, @ErrorMessage, 1;
        END

    INSERT INTO [dbo].[NotifyJobs]
        ([ScheduledJobId], [JobName], [JobDescription], [Status], [JobCreateTime], [JobLastUpdateTime])
        OUTPUT inserted.Id INTO @NotifyJobId
        VALUES (@ScheduledJobId, @JobName, @JobDescription, 0, @UtcDateNow, @UtcDateNow)

    SELECT [Id], [ScheduledJobId], [JobName], [JobDescription], [Status], [JobCreateTime], [JobLastUpdateTime]
    FROM [dbo].[NotifyJobs]
    WHERE [Id] = @NotifyJobId

RETURN 0
