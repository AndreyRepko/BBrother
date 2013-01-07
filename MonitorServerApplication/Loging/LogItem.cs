using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorServerApplication.Loging
{
    public class LogItem
    {
        public DateTime Time;
        public string Message;
        public string IP;
        public LogItem(string msg, string ip)
        {
            Message = msg;
            IP = ip;
            Time = DateTime.Now;
        }
    }
}
