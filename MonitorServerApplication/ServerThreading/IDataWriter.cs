using MonitorServerApplication.Loging;
using MonitorServerApplication.Packets;
using MonitorServerApplication.PacketsDefinition;

namespace MonitorServerApplication.ServerThreading
{
    public interface IDataWriter
    {
        void Log(LogItem item);
        void Log(InfoMessage item);
        void SaveData(PacketData item);
        void SaveArchiveData(ArchivePacketData item);
    }
}
