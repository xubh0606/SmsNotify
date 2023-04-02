using Dapper;
using Microsoft.Data.SqlClient;
using SmsNotify.Database.DatabaseObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsNotify.Database
{
    public static class DatabaseHelper
    {
        private static string s_connectionString = "Server=tcp:sms-notify-sql-server.database.windows.net,1433;Initial Catalog=SmsNotifyDatabase;Persist Security Info=False;User ID=SmsManager;Password=***;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static async Task<List<ScheduledJob>> GetNotifyJobsToRunNow()
        {
            using (var connection = new SqlConnection(s_connectionString))
            {
                string sprocName = StoredProcedureNames.sproc_GetNotifyJobsToRunNow;
                DynamicParameters parameters = new DynamicParameters();

                var scheduledJobs = await connection.QueryAsync<ScheduledJob>(sprocName, parameters, commandType: CommandType.StoredProcedure);

                return scheduledJobs.ToList();
            }
        }

        public static async Task UpdateScheduledJobNextJobDueTime(long scheduledJobId, DateTime nextJobDueTime)
        {
            using (var connection = new SqlConnection(s_connectionString))
            {
                string sprocName = StoredProcedureNames.sproc_UpdateScheduledJobNextJobDueTime;
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@ScheduledJobId", scheduledJobId, DbType.Int64, ParameterDirection.Input);
                parameters.Add("@NextJobDueTime", nextJobDueTime, DbType.DateTime2, ParameterDirection.Input);

                var rowsAffected = await connection.ExecuteAsync(sprocName, parameters, commandType: CommandType.StoredProcedure);

                return;
            }
        }
    }
}
