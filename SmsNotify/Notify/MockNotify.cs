using SmsNotify.Database.DatabaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SmsNotify.Notify
{
    public class MockNotify : INotify
    {
        public void Notify(NotifyJob notifyJob)
        {
            Console.WriteLine($"MockNotify sending notify with notifyId:{notifyJob.Id}, jobName:{notifyJob.JobName}, JobDescription:{notifyJob.JobDescription}");
        }
    }
}
