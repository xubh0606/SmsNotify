using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmsNotify.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsNotify.Database.Tests
{
    [TestClass()]
    public class DatabaseHelperTests
    {
        [TestMethod()]
        public async Task GetNotifyJobsToRunNowTest()
        {
            var result = await DatabaseHelper.GetNotifyJobsToRunNow();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
    }
}