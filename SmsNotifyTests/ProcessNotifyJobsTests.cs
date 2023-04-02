using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmsNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmsNotify.Database.DatabaseObjects;

namespace SmsNotify.Tests
{
    [TestClass()]
    public class ProcessNotifyJobsTests
    {
        [TestMethod()]
        public void WeekInMonthTest()
        {
            DateTime time1 = new DateTime(2023, 3, 19);
            Assert.AreEqual(4, TimeCalculation.WeekInMonth(time1));
        }

        [TestMethod()]
        public void WeekInMonthTest2()
        {
            DateTime time1 = new DateTime(2023, 3, 19);
            DateTime time2 = time1.AddDays(14);
            Assert.AreEqual(2, TimeCalculation.WeekInMonth(time2));
        }

        [TestMethod()]
        public void CalculateNextDueTimeTest()
        {
            ScheduledJob job1 = new ScheduledJob
            {
                StartDatetimeUtc = new DateTime(2023, 3, 19),
                EndDatetimeUtc = DateTime.Now.AddDays(30),
                JobType = ScheduledJobType.DailyJob,
                DaysOfWeek = 3,
                Enable = true,
                NextJobDueTimeUtc = DateTime.Now.AddDays(-1)
            };
            Assert.IsNotNull(TimeCalculation.CalculateNextDueTime(job1, true));
        }
        [TestMethod()]
        public void CalculateNextDueTimeTest2()
        {
            ScheduledJob job1 = new ScheduledJob
            {
                StartDatetimeUtc = new DateTime(2023, 3, 19),
                EndDatetimeUtc = DateTime.Now.AddDays(30),
                JobType = ScheduledJobType.DailyJob,
                DaysOfWeek = 3,
                Enable = true,
                NextJobDueTimeUtc = DateTime.Now.AddDays(-1)
            };
            Assert.AreEqual(TimeCalculation.DisabledDateTime, TimeCalculation.CalculateNextDueTime(job1, true));
        }
    }
}