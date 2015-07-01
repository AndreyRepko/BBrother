using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using MonitorServerApplication.DB;
using MonitorServerApplication.Loging;
using MonitorServerApplication.PacketsDefinition;

namespace MonitorServerApplication.ServerThreading
{
    public class ServerWriter : IDataWriter, IDataGetter
    {
        private readonly ConcurrentQueue<PacketData> _packets;
        private readonly ConcurrentQueue<InfoMessage> _infoMessages;
        private readonly ConcurrentQueue<LogItem> _logItems;
        private readonly DatabaseWorker _worker;

        // Thread signal. 
        private static ManualResetEvent _newItemEvent;

        public ServerWriter()
        {
            _packets = new ConcurrentQueue<PacketData>();
            _infoMessages = new ConcurrentQueue<InfoMessage>();
            _logItems = new ConcurrentQueue<LogItem>();
            _worker = new DatabaseWorker();
            _newItemEvent = new ManualResetEvent(false);
        }

        public void Log(LogItem item)
        {
            _logItems.Enqueue(item);
            _newItemEvent.Set();
        }

        public void Log(InfoMessage item)
        {
            _infoMessages.Enqueue(item);
            _newItemEvent.Set();
        }

        public void SaveData(PacketData item)
        {
            _packets.Enqueue(item);
            _newItemEvent.Set();
        }

        public void SaveArchiveData(ArchivePacketData item)
        {
            throw new NotImplementedException();
        }

        public void Save(CancellationToken ct)
        {

            while (!ct.IsCancellationRequested)
            {
                _newItemEvent.Reset();
                
                SaveAllData();
                
                _newItemEvent.WaitOne(TimingConstants.DefaultWaitTime);
            }
            var t = new Stopwatch();
            t.Start();
            
            SaveAllData();

            t.Stop();

            if (t.ElapsedMilliseconds > TimingConstants.MaxTimeToWait)
            {
                _logItems.Enqueue(
                    new LogItem(string.Format("Saving timeout: spent {0} and max value is {1}", t.ElapsedMilliseconds, TimingConstants.MaxTimeToWait),
                                "Server"));
                SaveAllData();
            }
        }

        private void SaveAllData()
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
