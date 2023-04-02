using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmsNotify.Database.DatabaseObjects;
using System.Diagnostics;

namespace SmsNotify
{
    public static class TimeCalculation
    {
        public static DateTime DisabledDateTime = new DateTime(9999,1,1);
        public static DateTime CalculateNextDueTime(ScheduledJob scheduledJob, bool IsRuning)
        {
            //计算前排除明确不需要进行的项目
            if (scheduledJob.Enable == false)
            {
                return DisabledDateTime;
            }
            if (IsRuning && scheduledJob.JobType == ScheduledJobType.OneTimeJob)
            {
                return DisabledDateTime;
            }
            DateTime lastScheduledTime = scheduledJob.NextJobDueTimeUtc;
            DateTime start = scheduledJob.StartDatetimeUtc > scheduledJob.NextJobDueTimeUtc ? scheduledJob.StartDatetimeUtc : scheduledJob.NextJobDueTimeUtc;
            //DateTime start = scheduledJob.StartDatetimeUtc > DateTime.Now ? scheduledJob.StartDatetimeUtc : DateTime.Now;
            //开始进行下一次计算，可能情况为：有下一次，分别是1-5，有没有下一次，可能因为时间超出end
            IList<TimeSpan> timeSpans;
            switch (scheduledJob.JobType)
            {
                case ScheduledJobType.OneTimeJob:
                    if (lastScheduledTime < scheduledJob.EndDatetimeUtc)
                    {
                        return lastScheduledTime;
                    }
                    return DisabledDateTime;

                case ScheduledJobType.DailyJob:
                    timeSpans = GetTimeList(scheduledJob.TimeArray);
                    if(timeSpans.Count == 0)
                    {
                        return DisabledDateTime;
                    }
                    return FindNextScheduledTime(timeSpans, start, scheduledJob.EndDatetimeUtc);

                case ScheduledJobType.WeeklyJob:
                    timeSpans = GetTimeList(scheduledJob.TimeArray);
                    if (timeSpans.Count == 0)
                    {
                        return DisabledDateTime;
                    }
                    return FindNextScheduledTime(timeSpans, start, scheduledJob.EndDatetimeUtc, scheduledJob.DaysOfWeek);

                case ScheduledJobType.MonthlyJob:
                    timeSpans = GetTimeList(scheduledJob.TimeArray);
                    if (timeSpans.Count == 0)
                    {
                        return DisabledDateTime;
                    }
                    return FindNextScheduledTime(timeSpans, start, scheduledJob.EndDatetimeUtc, scheduledJob.WeeksOfMonth, scheduledJob.DaysOfWeek);

                case ScheduledJobType.YearlyJob:
                    timeSpans = GetTimeList(scheduledJob.TimeArray);
                    if (timeSpans.Count == 0)
                    {
                        return DisabledDateTime;
                    }
                    return FindNextScheduledTime(timeSpans, start, scheduledJob.EndDatetimeUtc, scheduledJob.MonthsOfYear, scheduledJob.WeeksOfMonth, scheduledJob.DaysOfWeek);
            }
            throw new Exception("unreachable code");
        }
        //返回对应周数第一天的日期
        public static DateTime GoToWeeksOfMonth(DateTime start, int weeksOfMonth)
        {
            int week = WeekInMonth(start);
            start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
            while(start.DayOfWeek != 0)
            {
                start = start.AddDays(1);
            }
            week = WeekInMonth(start);
            while(((1 << week) & weeksOfMonth) == 0){
                start = start.AddDays(1);
                week = WeekInMonth(start);
            }
            return start;
        }
        //找到第一个符合时间要求的时间点，要求为月份
        public static DateTime GoToMonthsOfYear(DateTime start, int monthsOfYear)
        {
            int addYear = 0;
            int month = start.Month + 1;
            while(((1 << month) & monthsOfYear) == 0)
            {
                if(month == 13)
                {
                    month = 1;
                    addYear = 1;
                }
                else
                {
                    month++;
                }
            }
            return new DateTime(start.Year + addYear, month, 1, 0, 0, 0);
        }

        //找到第一个符合时间要求的时间点，要求为周数
        public static DateTime GoToDaysOfWeek(DateTime start, int daysOfWeek)
        {
            start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
            start = start.AddDays(1);
            while(((1 << (int)start.DayOfWeek) & daysOfWeek) == 0)
            {
                start = start.AddDays(1);
            }
            return start;
        }
        //找到下一个时间点，满足超出起始点并且不超过结尾时间
        public static DateTime FindNextScheduledTime(IList<TimeSpan> timeSpans, DateTime start, DateTime end, int monthsOfYear, int weeksOfMonth, int daysOfWeek)
        {
            //排除数字存在错误的问题
            if (monthsOfYear % 2 == 1)
            {
                throw new ArgumentException();
            }
            if (daysOfWeek < 1 || daysOfWeek > 127)
            {
                throw new ArgumentException();
            }
            if (weeksOfMonth < 2 || weeksOfMonth > 126 || weeksOfMonth % 2 == 1)
            {
                throw new ArgumentException();
            }
            bool isChanged = false;
            while (true)
            {
                if (((1 << start.Month) & monthsOfYear) == 0)
                {
                    start = GoToMonthsOfYear(start, monthsOfYear);
                    isChanged = true;
                    continue;
                }
                if (((1 << WeekInMonth(start)) & weeksOfMonth) == 0)
                {
                    start = GoToWeeksOfMonth(start, weeksOfMonth);
                    isChanged = true;
                    continue;
                }
                if (((1 << (int)start.DayOfWeek) & daysOfWeek) == 0)
                {
                    start = GoToDaysOfWeek(start, daysOfWeek);
                    isChanged = true;
                    continue;
                }
                break;
            }
            if (isChanged)
            {
                DateTime NextScheduledTime = GoToRightTime(timeSpans, start);
                if(NextScheduledTime < end)
                {
                    return NextScheduledTime;
                }
            }
            else
            {
                DateTime NextScheduledTime = GoToRightTime(timeSpans, start);
                if(NextScheduledTime == DisabledDateTime)
                {
                    start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                    start = start.AddDays(1);
                    while (true)
                    {
                        if (((1 << start.Month) & monthsOfYear) == 0)
                        {
                            start = GoToMonthsOfYear(start, monthsOfYear);
                            continue;
                        }
                        if (((1 << WeekInMonth(start)) & weeksOfMonth) == 0)
                        {
                            start = GoToWeeksOfMonth(start, weeksOfMonth);
                            continue;
                        }
                        if (((1 << (int)start.DayOfWeek) & daysOfWeek) == 0)
                        {
                            start = GoToDaysOfWeek(start, daysOfWeek);
                            continue;
                        }
                        break;
                    }
                    NextScheduledTime = GoToRightTime(timeSpans, start);
                }
                if (NextScheduledTime < end)
                {
                    return NextScheduledTime;
                }
            }
            return DisabledDateTime;
        }

        public static DateTime FindNextScheduledTime(IList<TimeSpan> timeSpans, DateTime start, DateTime end, int weeksOfMonth, int daysOfWeek)
        {
            //排除数字存在错误的问题
            if (daysOfWeek < 1 || daysOfWeek > 127)
            {
                throw new ArgumentException();
            }
            if (weeksOfMonth < 2 || weeksOfMonth > 126 || weeksOfMonth % 2 == 1)
            {
                throw new ArgumentException();
            }
            bool isChanged = false;
            while (true)
            {
                if (((1 << WeekInMonth(start)) & weeksOfMonth) == 0)
                {
                    start = GoToWeeksOfMonth(start, weeksOfMonth);
                    isChanged = true;
                    continue;
                }
                if (((1 << (int)start.DayOfWeek) & daysOfWeek) == 0)
                {
                    start = GoToDaysOfWeek(start, daysOfWeek);
                    isChanged = true;
                    continue;
                }
                break;
            }
            if (isChanged)
            {
                DateTime NextScheduledTime = GoToRightTime(timeSpans, start);
                if (NextScheduledTime < end)
                {
                    return NextScheduledTime;
                }
            }
            else
            {
                DateTime NextScheduledTime = GoToRightTime(timeSpans, start);
                if (NextScheduledTime == DisabledDateTime)
                {
                    start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                    start = start.AddDays(1);
                    while (true)
                    {
                        if (((1 << WeekInMonth(start)) & weeksOfMonth) == 0)
                        {
                            start = GoToWeeksOfMonth(start, weeksOfMonth);
                            continue;
                        }
                        if (((1 << (int)start.DayOfWeek) & daysOfWeek) == 0)
                        {
                            start = GoToDaysOfWeek(start, daysOfWeek);
                            continue;
                        }
                        break;
                    }
                    NextScheduledTime = GoToRightTime(timeSpans, start);
                }
                if (NextScheduledTime < end)
                {
                    return NextScheduledTime;
                }
            }
            return DisabledDateTime;
        }

        public static DateTime FindNextScheduledTime(IList<TimeSpan> timeSpans, DateTime start, DateTime end, int daysOfWeek)
        {
            //排除数字存在错误的问题
            if (daysOfWeek < 1 || daysOfWeek > 127)
            {
                throw new ArgumentException();
            }
            bool isChanged = false;
            while (true)
            {
                if (((1 << (int)start.DayOfWeek) & daysOfWeek) == 0)
                {
                    start = GoToDaysOfWeek(start, daysOfWeek);
                    isChanged = true;
                    continue;
                }
                break;
            }
            if (isChanged)
            {
                DateTime NextScheduledTime = GoToRightTime(timeSpans, start);
                if (NextScheduledTime < end)
                {
                    return NextScheduledTime;
                }
            }
            else
            {
                DateTime NextScheduledTime = GoToRightTime(timeSpans, start);
                if (NextScheduledTime == DisabledDateTime)
                {
                    start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                    start = start.AddDays(1);
                    while (true)
                    {
                        if (((1 << (int)start.DayOfWeek) & daysOfWeek) == 0)
                        {
                            start = GoToDaysOfWeek(start, daysOfWeek);
                            continue;
                        }
                        break;
                    }
                    NextScheduledTime = GoToRightTime(timeSpans, start);
                }
                if (NextScheduledTime < end)
                {
                    return NextScheduledTime;
                }
            }
            return DisabledDateTime;
        }

        public static DateTime FindNextScheduledTime(IList<TimeSpan> timeSpans, DateTime start, DateTime end)
        {
                DateTime NextScheduledTime = GoToRightTime(timeSpans, start);
                if (NextScheduledTime == DisabledDateTime)
                {
                    start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                    start = start.AddDays(1);
                    NextScheduledTime = GoToRightTime(timeSpans, start);
                }
                if (NextScheduledTime < end)
                {
                    return NextScheduledTime;
                }
            
            return DisabledDateTime;
        }

        public static DateTime GoToRightTime(IList<TimeSpan> timeSpans, DateTime start)
        {
            TimeSpan min_ts = new TimeSpan(-1);
            DateTime theDay = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
            TimeSpan tsStart = start.Subtract(theDay);
            for (int i = 0; i < timeSpans.Count; i++)
            {
                if (timeSpans[i] > tsStart)
                {
                    if (min_ts < tsStart)
                    {
                        min_ts = timeSpans[i];
                    }
                    else
                    {
                        if (timeSpans[i] < min_ts)
                        {
                            min_ts = timeSpans[i];
                        }
                    }
                }
            }
            if (min_ts.Ticks < 0)
            {
                return DisabledDateTime;
            }
            return theDay.Add(min_ts);
        }

        //public static DateTime GoToNextMonth(DateTime dateTime)
        //{
        //    if(dateTime.Month != 12)
        //    {
        //        return new DateTime(dateTime.Year, dateTime.Month + 1, 1, 0, 0, 0);
        //    }
        //    else
        //    {
        //        return new DateTime(dateTime.Year+1, 1, 1, 0, 0, 0);
        //    }
        //}
        public static int WeekInMonth(DateTime dateTime)
        {
            DateTime start = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
            int day = (int)start.DayOfWeek;
            return (dateTime.Day + day - 1) / 7 + 1;
        }
        public static IList<TimeSpan> GetTimeList(string timeArray)
        {
            if (string.IsNullOrEmpty(timeArray))
            {
                return new List<TimeSpan>();
            }
            string[] strs = timeArray.Split(new char[] { '[', ']' });
            IList<TimeSpan> sp = new List<TimeSpan>();
            for(int i = 0; i < strs.Length; i++)
            {
                if(strs[i] == "")
                {
                    continue;
                }
                IList<int> list = new List<int>();
                string[] strTime = strs[i].Split(':');
                if(strTime.Length < 3)
                {
                    throw new ArgumentException();
                }
                for(int j = 0; j < 3; j++)
                {
                    try 
                    {
                        list.Add(int.Parse(strTime[j]));
                    }
                    catch
                    {
                        throw new ArgumentException();
                    }
                }
                sp.Add(new TimeSpan(list[0], list[1], list[2]));
            }
            return sp;
        }
    }
}
