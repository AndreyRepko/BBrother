using System;
using System.Collections.Concurrent;
using System.Threading;
using MonitorServerApplication.DB;
using MonitorServerApplication.Loging;
using MonitorServerApplication.PacketsDefinition;
using MonitorServerApplication.Properties;

namespace MonitorServerApplication.ServerThreading
{
    public class ServerWriter : IDataWriter, IDataGetter
    {
        private readonly ConcurrentQueue<PacketData> _packets;
        private readonly ConcurrentQueue<InfoMessage> _infoMessages;
        private readonly ConcurrentQueue<LogItem> _logItems;
        private readonly DatabaseWorker _worker;

        public ServerWriter()
        {
            _packets = new ConcurrentQueue<PacketData>();
            _infoMessages = new ConcurrentQueue<InfoMessage>();
            _logItems = new ConcurrentQueue<LogItem>();
            _worker = new DatabaseWorker();
        }

        public void Log(LogItem item)
        {
            _logItems.Enqueue(item);
        }

        public void Log(InfoMessage item)
        {
            _infoMessages.Enqueue(item);
        }

        public void SaveData(PacketData item)
        {
            _packets.Enqueue(item);
        }

        public void SaveArchiveData(ArchivePacketData item)
        {
            throw new NotImplementedException();
        }

        public void Save(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (_logItems.Count != 0)
                {
                    LogItem item;
                    if (_logItems.TryDequeue(out item))
                    {
                        _worker.SaveItem(item);

                        OnLogItemSaveEvent(new LogItemEventArgs(item));
                    }
                }
            }
        }
        public event EventHandler<LogItemEventArgs> LogItemSaveEvent;

        private void OnLogItemSaveEvent(LogItemEventArgs e)
        {
            EventHandler<LogItemEventArgs> handler = LogItemSaveEvent;
            if (handler != null) handler(this, e);
        }

        public ClientSettings GetSettings(SettingsType settingsType)
        {
            return _worker.GetSettings(settingsType);
        }
    }
}
