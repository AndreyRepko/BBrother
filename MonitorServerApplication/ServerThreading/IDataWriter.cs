using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonitorServerApplication.Loging;
using MonitorServerApplication.PacketsDefinition;

namespace MonitorServerApplication.DB
{
    public interface IDataWriter
    {
        void Log(LogItem item);
        void Log(InfoMessage item);
        void SaveData(PacketData item);
        void SaveArchiveData(ArchivePacketData item);
    }
}
