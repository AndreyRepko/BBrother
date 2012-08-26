using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MonitorServerApplication
{
    public class ServerMainThread
    {
        TcpListener _listener; // Объект, принимающий TCP-клиентов
        private Task someTask ;
        public bool IsNeedToStop = false;
        public ServerMainThread(int port)
        {
            _listener = new TcpListener(IPAddress.Any,port);
            //33, 34
        }

        static void ClientThread(Object StateInfo)
        {
            new Client((TcpClient)StateInfo);
        }

        void DoAcceptConnectionsAsync()
        {
            while (!IsNeedToStop)
            {
                if (_listener.Pending())
                {
                    // Принимаем нового клиента
                    var ClientThread1 = _listener.AcceptTcpClient();
                    // Создаем поток
                    Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                    // И запускаем этот поток, передавая ему принятого клиента
                    Thread.Start(ClientThread1);;
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        async Task DoAcceptConnections()
        {
            someTask = Task.Factory.StartNew(() => DoAcceptConnectionsAsync());
            await someTask; 
        }

        public async void Start()
        {
            _listener.Start();
            await DoAcceptConnections();
        }

        public void Stop()
        {
            IsNeedToStop = true;
            someTask.Wait(10000);

            if (!someTask.IsCompleted)
            {
                throw new System.ArgumentException("Stoping timeout!"); 
            }
            _listener.Stop();
        }
    }
}
