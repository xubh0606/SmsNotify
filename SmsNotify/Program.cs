using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmsNotify
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            Console.WriteLine("Start");

            ProcessNotifyJobs processNotifyJobs = new ProcessNotifyJobs();
            List<Task> tasks = new List<Task>();

            var processScheduledNotifyJobsTask = processNotifyJobs.ProcessScheduledNotifyJobs();
            var processNotifyJobInstancesTask = processNotifyJobs.ProcessNotifyJobInstances();

            tasks.Add(processScheduledNotifyJobsTask);
            tasks.Add(processNotifyJobInstancesTask);

            await Task.WhenAll(tasks);

            Console.WriteLine("Finished, why am I here....");
        }
    }
}
