using SmsNotify.Database.DatabaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsNotify.Notify
{
    public interface INotify
    {
        void Notify(NotifyJob notifyJob);
    }
}
