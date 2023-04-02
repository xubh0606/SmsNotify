using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsNotify.Database
{
    internal class StoredProcedureNames
    {
        public static string sproc_GetNotifyJobsToRunNow = "GetNotifyJobsToRunNow_V0";
        public static string sproc_UpdateScheduledJobNextJobDueTime = "UpdateScheduledJobNextJobDueTime_V0";
        public static string sproc_InsertNotifyJob = "InsertNotifyJob_V0";

        // for future
        public static string sproc_CreateScheduledJob = "CreateScheduledJob_V0";
        public static string sproc_UpdateScheduledJob = "UpdateScheduledJob_V0";

    }

}
