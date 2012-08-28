using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace MonitorServerApplication
{
    // Класс-обработчик клиента
    internal class Client
    {
        private NetworkStream _client;
        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public Client(TcpClient Client)
        {
            _client = Client.GetStream();

            _client.ReadTimeout = 1000;

            var signature = new byte[4];
            _client.Read(signature, 0, 4);

            switch (BitConverter.ToInt32(signature, 0))
            {
                case OldProtocolConst.Con_Begin:
                    var read_buffer = new byte[220];
                    _client.Read(read_buffer, 0, 220);
                    var pc_name = System.Text.Encoding.GetEncoding(1251).GetString(read_buffer, 0, 100);
                    var ip = System.Text.Encoding.GetEncoding(1251).GetString(read_buffer, 100, 20);
                    var user_name = System.Text.Encoding.GetEncoding(1251).GetString(read_buffer, 120, 100);
                    //string s = new string(read_buffer, 0, 100);
                    break;
                case OldProtocolConst.Con_End:
                    break;
                default:

                    break;
            }
            // Закроем соединение
            Client.Close();
        }
        
        
    }
}
