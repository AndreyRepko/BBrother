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
        private Task _writeToDBTask;

        public bool IsNeedToStop = false;

        private ClientList _clients;
        public ServerWriter _DBWriter;
        public ServerMainThread(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            //33, 34
        }

        void DoAcceptConnectionsAsync()
        {
            _DBWriter.Log(new LogItem("Server is starting now", "no ip"));

            while (!IsNeedToStop)
            {
                if (_listener.Pending())
                {
                    // Принимаем нового клиента
                    var clientThread1 = _listener.AcceptTcpClient();
                    _DBWriter.Log(new LogItem("New client is coming!", ((IPEndPoint)clientThread1.Client.RemoteEndPoint).Address.ToString()));
                    var currentClient = new ClientThread(clientThread1, ref _DBWriter);
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

            /*while ((_DBWriter._packets.Count != 0) || (_DBWriter.InfoMessages.Count != 0))
            {
                Thread.Sleep(10);
            }*/

        }

        private void StartDBWriter()
        {
            while (!IsNeedToStop)
            {
                _DBWriter.Save();
                Thread.Sleep(10);
            }            
        }

        async Task DoCreateDBWriter()
        {
            _writeToDBTask = Task.Factory.StartNew(DoAcceptConnectionsAsync);
            await _writeToDBTask;            
        }

        async Task DoAcceptConnections()
        {
            _someTask = Task.Factory.StartNew(DoAcceptConnectionsAsync);
            await _someTask;
        }

        public async void StartClients()
        {
            _listener.Start();
            _clients = new ClientList();
            _DBWriter = new ServerWriter();

            await DoAcceptConnections();
        }

        public async void StartWriter()
        {
            await DoCreateDBWriter();
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
