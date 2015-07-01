using System;

namespace MonitorServerApplication.Packets
{
    public class InfoMessage
    {
        public int MessageType;
        public DateTime Time;
        public string IP;
        public string UserName;
        public string Info;
    }
}
