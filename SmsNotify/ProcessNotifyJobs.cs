using SmsNotify.Database;
using SmsNotify.Database.DatabaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsNotify
{
    public class ProcessNotifyJobs
    {
        // 1. get all scheduled notify jobs that are due
        // 2. create new notify job instance, status = NotStarted
        // 3. Update scheduled notify job [NextJobDueTime] according to each job
        public async Task ProcessScheduledNotifyJobs()
        {
            while(true)
            {
                try
                {
                    var scheduledJobs = await DatabaseHelper.GetNotifyJobsToRunNow();

                    foreach (var scheduledJob in scheduledJobs)
                    {
                        // step 1: calculate and update next schedule time for the scheduled job
                        var nextJobDueTime = TimeCalculation.CalculateNextDueTime(scheduledJob, true);
                        if(nextJobDueTime != TimeCalculation.DisabledDateTime)
                        {
                            await DatabaseHelper.UpdateScheduledJobNextJobDueTime(scheduledJob.Id, nextJobDueTime);
                        }
                        // step 2: create a new NotifyJob record, so it will be pick up by <ProcessNotifyJobInstances>
                        //
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ProcessScheduledNotifyJobs failed with error : {ex}");
                }

                await Task.Delay(5000);
            }
        }


        // 1. get all notify job instances that are not completed ( NotStarted )
        // 2. notify via Sms
        // 3. update job instance status to be completed / failed
        public async Task ProcessNotifyJobInstances()
        {
            while (true)
            {
                try
                {
                    await Task.Yield();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ProcessNotifyJobInstances failed with error : {ex}");
                }
                await Task.Delay(10000);
            }
        }
    }
}
