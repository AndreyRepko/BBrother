using System.Collections.Concurrent;
using MonitorServerApplication.DB;
using MonitorServerApplication.Loging;
using MonitorServerApplication.Packets;

namespace MonitorServerApplication.ServerThreading
{
    public class ServerWriter
    {
        private readonly ConcurrentQueue<PacketData> _packets;
        private readonly ConcurrentQueue<InfoMessage> _infoMessages;
        private readonly ConcurrentQueue<LogItem> _logItems;
        private readonly DatabaseWriter _writer;

        public ServerWriter()
        {
            _packets = new ConcurrentQueue<PacketData>();
            _infoMessages = new ConcurrentQueue<InfoMessage>();
            _logItems = new ConcurrentQueue<LogItem>();
            _writer = new DatabaseWriter();
        }

        public void Log(LogItem item)
        {
            _logItems.Enqueue(item);
        }

        public void Log(InfoMessage item)
        {
            _infoMessages.Enqueue(item);
        }

        public void Log(PacketData item)
        {
            _packets.Enqueue(item);
        }

        public void Save()
        {
            if (_logItems.Count != 0)
            {
                LogItem item;
                if (_logItems.TryDequeue(out item))
                {
                    _writer.SaveItem(item);
                }
            }
        }
    }
}
