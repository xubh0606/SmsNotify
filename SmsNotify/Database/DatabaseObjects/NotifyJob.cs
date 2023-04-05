using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsNotify.Database.DatabaseObjects
{
    public enum NotifyJobStatus
    {
        NotStarted = 0,
        Running = 1,
        Completed = 2,
        Failed = 3,
    }
    public class NotifyJob
    {
        public long Id { get; set; }

        public long ScheduledJobId { get; set; }

        public string JobName { get; set; }
        
        public string JobDescription { get; set; }

        public NotifyJobStatus Status { get; set; }
        
        public DateTime JobCreateTime { get; set; }

        public DateTime JobLastUpdateTime { get; set; }
    }
}
