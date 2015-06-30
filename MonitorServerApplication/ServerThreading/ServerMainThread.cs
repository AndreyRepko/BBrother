using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MonitorServerApplication.DB;
using MonitorServerApplication.Loging;

namespace MonitorServerApplication.ServerThreading
{
    public static class ServerMainThread
    {
        private const int Timeouts = 30 * 1000;

        // Thread signal. 
        private static ManualResetEvent _tcpClientConnected;

        public static void DoAcceptConnections(int port, IDataWriter writer, IDataGetter reader, CancellationToken ct)
        {
            writer.Log(new LogItem("Server is starting now", "no ip"));
            var listener = new TcpListener(IPAddress.Any, port);
            _tcpClientConnected =  new ManualResetEvent(false);

            try
            {
                listener.Start();
                writer.Log(new LogItem("Server is started and begin to listen", "All IP"));
                
                var connected = true;

                while (!ct.IsCancellationRequested)
                {
                    if (connected)
                    {
                        _tcpClientConnected.Reset();
                        listener.BeginAcceptTcpClient(
                            ar => DoAcceptTcpClientCallback(ar, writer, reader, ct),
                            listener);
                    }

                    connected = _tcpClientConnected.WaitOne(100);
                }

            }
            catch (SocketException e)
            {
                writer.Log(new LogItem("Server got an socket error: " + e.Message, "no ip"));
            }
            finally
            {
                listener.Stop();
                writer.Log(new LogItem("Server is terminated", "no ip"));
            }
        }

        // Process the client connection. 
        private static void DoAcceptTcpClientCallback(IAsyncResult ar, IDataWriter writer, IDataGetter reader, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                return;
            // Get the listener that handles the client request.
            var listener = (TcpListener)ar.AsyncState;

            // End the operation
            var client = listener.EndAcceptTcpClient(ar);

            client.ReceiveTimeout = Timeouts;
            client.SendTimeout = Timeouts;
            writer.Log(new LogItem("New client is coming!", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()));
            try
            {
                var currentClient = new ClientThread(client, writer, reader);
                //
                Task.Run(() => currentClient.Execute(ct), ct);
            }
            catch (Exception e)
            {
                writer.Log(new LogItem("Server got an exception when client arrived: " + e.Message, "no ip"));
            }
           
            // Signal the calling thread to continue.
            _tcpClientConnected.Set();

        }
    }
}
