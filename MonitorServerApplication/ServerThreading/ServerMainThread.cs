using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MonitorServerApplication.Loging;
using MonitorServerApplication.Packets;

namespace MonitorServerApplication.ServerThreading
{
    public class ServerMainThread
    {
        readonly TcpListener _listener; // Объект, принимающий TCP-клиентов
        private Task _someTask;
        public bool IsNeedToStop = false;
        private ClientList _clients;
        public ServerMainThread(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            //33, 34
        }

        public ConcurrentQueue<PacketData> Packets;
        public ConcurrentQueue<InfoMessage> InfoMessages;
        public ConcurrentQueue<LogItem> LogItems;

        void DoAcceptConnectionsAsync()
        {
            Packets = new ConcurrentQueue<PacketData>();
            InfoMessages = new ConcurrentQueue<InfoMessage>();
            LogItems = new ConcurrentQueue<LogItem> ();

            LogItems.Enqueue(new LogItem("Server is starting now","no ip"));

            while (!IsNeedToStop)
            {
                if (_listener.Pending())
                {
                    // Принимаем нового клиента
                    var clientThread1 = _listener.AcceptTcpClient();
                    LogItems.Enqueue(new LogItem("New client is coming!", ((IPEndPoint)clientThread1.Client.RemoteEndPoint).Address.ToString()));
                    var currentClient = new ClientThread(clientThread1, ref Packets, ref InfoMessages, ref LogItems);
                    // Создаем поток
                    var thread = new Thread(currentClient.Execute);
                    // И запускаем этот поток, передавая ему принятого клиента
                    thread.Start();
                    _clients.Add(currentClient);
                    _clients.RemoveInnactive();
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            _clients.Stop();

            while (_clients.Count != 0)
            {
                _clients.RemoveInnactive();
                Thread.Sleep(10);
            }

            while ((Packets.Count != 0) || (InfoMessages.Count !=0))
            {
                Thread.Sleep(10);
            }

        }

        async Task DoAcceptConnections()
        {
            _someTask = Task.Factory.StartNew(DoAcceptConnectionsAsync);
            await _someTask;
        }

        public async void Start()
        {
            _listener.Start();
            _clients = new ClientList();
            await DoAcceptConnections();
        }

        public void Stop()
        {
            IsNeedToStop = true;
            _someTask.Wait(10000);

            if (!_someTask.IsCompleted)
            {
                throw new System.ArgumentException("Stoping timeout!");
            }
            _listener.Stop();
        }
    }
}
