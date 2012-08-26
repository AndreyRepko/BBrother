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
        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public Client(TcpClient Client)
        {
            // Код простой HTML-странички
            const string Html = "<html><body><h1>It works!</h1></body></html>";
            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            var Str = "HTTP/1.1 200 OK\nContent-type: text/html\nContent-Length:" + Html.Length.ToString() + "\n\n" +
                         Html;
            // Приведем строку к виду массива байт
            var Buffer = Encoding.ASCII.GetBytes(Str);
            // Отправим его клиенту
            Client.GetStream().Write(Buffer, 0, Buffer.Length);
            // Закроем соединение
            Client.Close();
        }
        
        
    }
}
