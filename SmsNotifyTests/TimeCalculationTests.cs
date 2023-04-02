using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmsNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmsNotify.Database.DatabaseObjects;
using System.Diagnostics;

namespace SmsNotify.Tests
{
    [TestClass()]
    public class TimeCalculationTests
    {
        [TestMethod()]
        public void CalculateNextDueTimeTest_EmptyList()
        {
            ScheduledJob testJob = new ScheduledJob()
            {
                StartDatetimeUtc = new DateTime(2023, 3, 19),
                TimeArray = "",
                EndDatetimeUtc = DateTime.Now.AddDays(30),
                JobType = ScheduledJobType.DailyJob,
                DaysOfWeek = 3,
                Enable = true,
                NextJobDueTimeUtc = DateTime.Now.AddDays(1)
            };
            Assert.IsTrue(TimeCalculation.DisabledDateTime == TimeCalculation.CalculateNextDueTime(testJob, false));
        }

        [TestMethod()]
        public void CalculateNextDueTimeTest_Daily()
        {
            DateTime now = new DateTime(2023, 4, 1, 4, 4, 4);
            ScheduledJob testJob = new ScheduledJob()
            {
                StartDatetimeUtc = new DateTime(2023, 3, 19),
                TimeArray = "[12:24:00]",
                EndDatetimeUtc = now.AddDays(30),
                JobType = ScheduledJobType.DailyJob,
                DaysOfWeek = 3,
                Enable = true,
                NextJobDueTimeUtc = new DateTime(2023, 4, 1, 12, 24, 00)
        };
            IList<DateTime> list = new List<DateTime>();
            while(testJob.NextJobDueTimeUtc != TimeCalculation.DisabledDateTime)
            { 
                testJob.NextJobDueTimeUtc = TimeCalculation.CalculateNextDueTime(testJob, false);
                list.Add(testJob.NextJobDueTimeUtc);
            }
            Assert.AreEqual(30, list.Count);
        }

        [TestMethod()]
        public void CalculateNextDueTimeTest_Daily2()
        {
            DateTime now = new DateTime(2023, 4, 1, 4, 4, 4);
            ScheduledJob testJob = new ScheduledJob()
            {
                StartDatetimeUtc = new DateTime(2023, 3, 19),
                TimeArray = "[12:24:00][16:20:00]",
                EndDatetimeUtc = now.AddDays(30),
                JobType = ScheduledJobType.DailyJob,
                DaysOfWeek = 3,
                Enable = true,
                NextJobDueTimeUtc = new DateTime(2023, 4, 1, 12, 24, 00)
            };
            IList<DateTime> list = new List<DateTime>();
            while (testJob.NextJobDueTimeUtc != TimeCalculation.DisabledDateTime)
            {
                testJob.NextJobDueTimeUtc = TimeCalculation.CalculateNextDueTime(testJob, false);
                list.Add(testJob.NextJobDueTimeUtc);
            }
            Assert.AreEqual(60, list.Count);
        }

        [TestMethod()]
        public void CalculateNextDueTimeTest_Weekly()
        {
            DateTime now = new DateTime(2023, 4, 1, 4, 4, 4);
            ScheduledJob testJob = new ScheduledJob()
            {
                StartDatetimeUtc = new DateTime(2023, 4, 10),
                TimeArray = "[12:24:00]",
                EndDatetimeUtc = now.AddDays(30),
                JobType = ScheduledJobType.WeeklyJob,
                DaysOfWeek = 18,
                WeeksOfMonth = 18,
                Enable = true,
                NextJobDueTimeUtc = new DateTime(2023, 4, 1, 12, 24, 00)
            };
            IList<DateTime> list = new List<DateTime>();
            while (testJob.NextJobDueTimeUtc != TimeCalculation.DisabledDateTime)
            {
                testJob.NextJobDueTimeUtc = TimeCalculation.CalculateNextDueTime(testJob, false);
                list.Add(testJob.NextJobDueTimeUtc);
            }
            Assert.AreEqual(7, list.Count);
        }

        [TestMethod()]
        public void CalculateNextDueTimeTest_Monthly()
        {
            DateTime now = new DateTime(2023, 4, 1, 4, 4, 4);
            ScheduledJob testJob = new ScheduledJob()
            {
                StartDatetimeUtc = new DateTime(2023, 4, 10),
                TimeArray = "[12:24:00]",
                EndDatetimeUtc = now.AddYears(1),
                JobType = ScheduledJobType.MonthlyJob,
                DaysOfWeek = 18,
                WeeksOfMonth = 22,
                Enable = true,
                NextJobDueTimeUtc = new DateTime(2023, 4, 1, 12, 24, 00)
            };
            IList<DateTime> list = new List<DateTime>();
            while (testJob.NextJobDueTimeUtc != TimeCalculation.DisabledDateTime)
            {
                testJob.NextJobDueTimeUtc = TimeCalculation.CalculateNextDueTime(testJob, false);
                list.Add(testJob.NextJobDueTimeUtc);
            }
            Assert.AreEqual(54, list.Count);
        }

        [TestMethod()]
        public void CalculateNextDueTimeTest_Yearly()
        {
            DateTime now = new DateTime(2023, 4, 1, 4, 4, 4);
            ScheduledJob testJob = new ScheduledJob()
            {
                StartDatetimeUtc = new DateTime(2023, 4, 10),
                TimeArray = "[12:24:00][3:10:20]",
                EndDatetimeUtc = now.AddYears(3),
                JobType = ScheduledJobType.YearlyJob,
                DaysOfWeek = 18,
                WeeksOfMonth = 18,
                MonthsOfYear = 146,
                Enable = true,
                NextJobDueTimeUtc = new DateTime(2023, 4, 1, 12, 24, 00)
            };
            IList<DateTime> list = new List<DateTime>();
            while (testJob.NextJobDueTimeUtc != TimeCalculation.DisabledDateTime)
            {
                testJob.NextJobDueTimeUtc = TimeCalculation.CalculateNextDueTime(testJob, false);
                list.Add(testJob.NextJobDueTimeUtc);
            }
            Assert.AreEqual(57, list.Count);
        }

        [TestMethod()]
        public void CalculateNextDueTimeTest_DaylyJob1()
        {
            DateTime now = new DateTime(2023, 3, 24, 1, 1, 1);
            ScheduledJob testJob = new ScheduledJob()
            {
                StartDatetimeUtc = new DateTime(2023, 3, 19),
                EndDatetimeUtc = now.AddDays(30),
                JobType = ScheduledJobType.DailyJob,
                Enable = true,
                NextJobDueTimeUtc = now.AddDays(1)
            };
            DateTime t = TimeCalculation.CalculateNextDueTime(testJob, false);
            Assert.IsTrue(t != TimeCalculation.DisabledDateTime);
        }

        //[TestMethod()]
        //public void GoToNextMonthTest()
        //{
        //    DateTime target = new DateTime(2023, 4, 1, 0, 0, 0);
        //    DateTime now = new DateTime(2023, 3, 23, 1, 1, 1);
        //    Assert.IsTrue(target == TimeCalculation.GoToNextMonth(now));
        //}

        [TestMethod()]
        public void WeekInMonthTest_RandomTime()
        {
            DateTime now = new DateTime(2025, 3, 23, 1, 1, 1);
            Assert.AreEqual(4, TimeCalculation.WeekInMonth(now));
        }

        [TestMethod()]
        public void WeekInMonthTest_FirstWeek()
        {
            DateTime now = new DateTime(2023, 4, 1, 1, 1, 1);
            Assert.AreEqual(1, TimeCalculation.WeekInMonth(now));
        }

        [TestMethod()]
        public void WeekInMonthTest_SecondWeek()
        {
            DateTime now = new DateTime(2023, 4, 2, 1, 1, 1);
            Assert.AreEqual(2, TimeCalculation.WeekInMonth(now));
        }

        [TestMethod()]
        public void WeekInMonthTest_EndOfMonth()
        {
            DateTime now = new DateTime(2023, 3, 30, 1, 1, 1);
            Assert.AreEqual(5, TimeCalculation.WeekInMonth(now));
        }

        [TestMethod()]
        public void GetTimeListTest()
        {
            string TimeArray = "[12:30:12][12:30:12][12:30:12][12:30:12]";
            TimeSpan sp = new TimeSpan(12, 30, 12);
            Assert.AreEqual(sp, TimeCalculation.GetTimeList(TimeArray)[0]);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetTimeListTest_InvalidFormat()
        {
            string TimeArray = "[12:30:12][12:30:12],[12:30:12],[12:30:12]";
            TimeCalculation.GetTimeList(TimeArray);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetTimeListTest_MissingNumber()
        {
            string TimeArray = "[12:30:12][12:30:12][12:30:12][12:3]";
            TimeCalculation.GetTimeList(TimeArray);
        }

        [TestMethod()]
        public void GetTimeListTest_EmptyList()
        {
            string TimeArray = "";
            Assert.AreEqual(0, TimeCalculation.GetTimeList(TimeArray).Count);
        }

        [TestMethod()]
        public void GoToWeeksOfMonthTest()
        {
            DateTime now = new DateTime(2023, 3, 23, 1, 1, 1);
            //当为零时应被排除，但是考虑时要加上
            int[] WeeksOfMonths = new int[] { 2, 4, 6, 8, 16};
            int[] expect = new int[] { 1, 2, 1, 9, 16 };
            for (int i = 0; i < WeeksOfMonths.Length; i++)
            {
                Assert.AreEqual(expect[i] ,TimeCalculation.GoToWeeksOfMonth(now, WeeksOfMonths[i]).Day);
            }
        }

        [TestMethod()]
        public void GoToMonthsOfYearTest()
        {
            IList<DateTime> list = new List<DateTime>();
            DateTime now = new DateTime(2023, 8, 23, 1, 1, 1);
            //当为零时应被排除，但是考虑时要加上
            list.Add(TimeCalculation.GoToMonthsOfYear(now, 146));
            
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod()]
        public void GoToDaysOfWeekTest()
        {
            IList<DateTime> list = new List<DateTime>();
            DateTime now = new DateTime(2023, 3, 23, 1, 1, 1);
            //当为零时应被排除，但是考虑时要加上
            for (int i = 1; i <= 127; i++)
            {
                list.Add(TimeCalculation.GoToDaysOfWeek(now, i));
            }
            Assert.AreEqual(127, list.Count);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GoToDaysOfWeekTest_OutOfRange()
        {
            DateTime now = new DateTime(2023, 3, 23, 1, 1, 1);
            //当为零时应被排除，但是考虑时要加上
            TimeCalculation.GoToDaysOfWeek(now, 0);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void FindNextScheduledTimeTest_NotInRange()
        {
            IList<TimeSpan> timeSpans = TimeCalculation.GetTimeList("[12:30:12][12:40:12][2:30:12][1:30:12]");
            DateTime now = new DateTime(2023, 3, 23, 1, 1, 1);
            Assert.IsTrue(TimeCalculation.DisabledDateTime != TimeCalculation.FindNextScheduledTime(timeSpans, now, now, 7, 127));
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void FindNextScheduledTimeTest_NotInRange2()
        {
            IList<TimeSpan> timeSpans = TimeCalculation.GetTimeList("[12:30:12][12:40:12][2:30:12][1:30:12]");
            DateTime now = new DateTime(2023, 3, 23, 1, 1, 1);
            Assert.IsTrue(TimeCalculation.DisabledDateTime != TimeCalculation.FindNextScheduledTime(timeSpans, now, now, 6, 127));
        }

        [TestMethod()]
        public void FindNextScheduledTimeTest_InSameDay()
        {
            IList<TimeSpan> timeSpans = TimeCalculation.GetTimeList("[12:30:12][12:40:12][2:30:12][1:30:12]");
            DateTime now = new DateTime(2023, 3, 23, 1, 1, 1);
            DateTime result = TimeCalculation.FindNextScheduledTime(timeSpans, now, now.AddDays(10));
            Assert.IsTrue(new DateTime(2023, 3, 23, 1, 30, 12) == result);
        }

        [TestMethod()]
        public void FindNextScheduledTimeTest_NotInSameDay()
        {
            IList<TimeSpan> timeSpans = TimeCalculation.GetTimeList("[12:30:12][12:40:12][2:30:12][1:30:12]");
            DateTime now = new DateTime(2023, 3, 23, 15, 1, 1);
            DateTime result = TimeCalculation.FindNextScheduledTime(timeSpans, now,  now.AddDays(1),(1 << 13) - 2, 126, 127);
            Assert.IsTrue(new DateTime(2023, 3, 24, 1, 30, 12) == result);
        }
    }
}