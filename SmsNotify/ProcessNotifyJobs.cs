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

        //public DateTime CalculateNextDueTime(ScheduledJob scheduledJob, bool IsRuning)
        //{
        //    //计算前排除明确不需要进行的项目
        //    if(scheduledJob.Enable == false)
        //    {
        //        return default;
        //    }
        //    if(IsRuning && scheduledJob.JobType == 1)
        //    {
        //        return default;
        //    }
        //    //DateTime[] dateTimes = GetDateTimes(scheduledJob.TimeArray);
        //    DateTime lastScheduledTime = scheduledJob.NextJobDueTime;
        //    DateTime start = scheduledJob.StartDatetime > DateTime.Now ? scheduledJob.StartDatetime : DateTime.Now;
        //    //开始进行下一次计算，可能情况为：有下一次，分别是1-5，没有下一次，可能因为时间超出end
        //    switch (scheduledJob.JobType)
        //    {
        //        case 1:
        //            if(lastScheduledTime < scheduledJob.EndDatetime)
        //            {
        //                return lastScheduledTime;
        //            }
        //            return default;
        //        case 2:
        //            lastScheduledTime.AddDays(start.Subtract(lastScheduledTime).Days+1);
        //            if(lastScheduledTime < scheduledJob.EndDatetime)
        //            {
        //                return lastScheduledTime;
        //            }
        //            return default;
        //        case 3:
        //            if(scheduledJob.DaysOfWeek == 0)
        //            {
        //                return default;
        //            }
        //            lastScheduledTime.AddDays(start.Subtract(lastScheduledTime).Days + 1);
        //            while(((scheduledJob.DaysOfWeek << (int)lastScheduledTime.DayOfWeek)&1) != 1)
        //            {
        //                lastScheduledTime.AddDays(1);
        //            }
        //            if (lastScheduledTime < scheduledJob.EndDatetime)
        //            {
        //                return lastScheduledTime;
        //            }
        //            return default;
        //        case 4:
        //            if (scheduledJob.WeeksOfMonth == 0)
        //            {
        //                return default;
        //            }
        //            lastScheduledTime.AddDays(start.Subtract(lastScheduledTime).Days + 1);
        //            while (((scheduledJob.WeeksOfMonth << WeekInMonth(lastScheduledTime)) & 1) != 1)
        //            {
        //                lastScheduledTime.AddDays(1);
        //            }
        //            if (lastScheduledTime < scheduledJob.EndDatetime)
        //            {
        //                return lastScheduledTime;
        //            }
        //            return default;
        //        case 5:
        //            if (scheduledJob.MonthsOfYear == 0)
        //            {
        //                return default;
        //            }
        //            lastScheduledTime.AddDays(start.Subtract(lastScheduledTime).Days + 1);
        //            while (((scheduledJob.MonthsOfYear << lastScheduledTime.Month) & 1) != 1)
        //            {
        //                lastScheduledTime = GoToNextMonth(lastScheduledTime);
        //            }
        //            if (lastScheduledTime < scheduledJob.EndDatetime)
        //            {
        //                return lastScheduledTime;
        //            }
        //            return default;
        //    }
        //    return scheduledJob.NextJobDueTime.AddMinutes(1);
        //}

        //public DateTime GoToNextMonth(DateTime dateTime)
        //{
        //    return new DateTime(dateTime.Year, dateTime.Month+1, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        //}
        //public int WeekInMonth(DateTime dateTime)
        //{
        //    DateTime start = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
        }

        //DateTime[] GetDateTimes(string TimeArray)
        //{
        //    string[] strs = TimeArray.Split(new char[] { '[', ']' });
        //    DateTime[] dateTimes = new DateTime[strs.Length];
        //    for(int i = 0; i < strs.Length; i++)
        //    {
        //        dateTimes[i] = DateTime.Parse(strs[i]);
        //    }
        //    return dateTimes;
        //}
    
}
