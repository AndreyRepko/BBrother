using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MonitorServerApplication.DB;
using MonitorServerApplication.Loging;
using MonitorServerApplication.Packets;

namespace MonitorServerApplication.ServerThreading
{
    public static class ServerMainThread
    {
        public static void DoAcceptConnectionsAsync(int port, IDataWriter writer, IDataGetter reader, CancellationToken ct)
        {
            writer.Log(new LogItem("Server is starting now", "no ip"));
            var listener = new TcpListener(IPAddress.Any, port);
            try
            {
                listener.Start();
                writer.Log(new LogItem("Server is started and begin to listen", "no ip"));

                while (!ct.IsCancellationRequested)
                {
                    if (listener.Pending())
                    {
                        // Accept new client connection
                        var clientThread1 = listener.AcceptTcpClient();
                        writer.Log(new LogItem("New client is coming!", ((IPEndPoint)clientThread1.Client.RemoteEndPoint).Address.ToString()));
                        try
                        {
                            var currentClient = new ClientThread(clientThread1, writer, reader);
                            // 
                            (new Task(() => currentClient.Execute(ct), ct)).Start();
                        }
                        catch (Exception e)
                        {
                            writer.Log(new LogItem("Server got an exception when client arrived: " + e.Message, "no ip"));  
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
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
    }
}
