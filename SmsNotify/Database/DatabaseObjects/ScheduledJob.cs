using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsNotify.Database.DatabaseObjects
{
    public enum ScheduledJobType
    {
        OneTimeJob = 1, 
        DailyJob, 
        WeeklyJob, 
        MonthlyJob, 
        YearlyJob
    };
    public class ScheduledJob
    {
        public long Id { get; set; }

        public ScheduledJobType JobType { get; set; }

        public string TimeArray { get; set; }

        public int DaysOfWeek { get; set; }
        public int WeeksOfMonth { get; set; }

        public int MonthsOfYear { get; set; }

        public DateTime StartDatetimeUtc { get; set; }

        public DateTime EndDatetimeUtc { get; set; }

        public bool Enable { get; set; }

        public string TimeZone { get; set; }

        public DateTime NextJobDueTimeUtc { get; set; }
    }
}
