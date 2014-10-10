using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MessageEception;

namespace Messenger
{
    class SocketClient
    {
        String domain;
        int port; 
        public SocketClient(String domain, int port)
        {
            this.domain = domain;
            this.port = port;
        }

        public void init()
        {
            try
            {
                SendMessageFromSocket(domain, port);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static void SendMessageFromSocket(String domain, int port)
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];

            // Соединяемся с удаленным устройством

            // Устанавливаем удаленную точку для сокета
            IPHostEntry ipHost = Dns.GetHostEntry(domain);
            IPAddress ipAddr;
            if (ipHost.AddressList.Length != 0)
            {
                ipAddr = ipHost.AddressList[0];
            }
            else
            {
                throw new IpNotFoundExecption("Ips not found");
            }
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            Console.Write("Введите сообщение: ");
            string message = Console.ReadLine();

            Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());
            byte[] msg = Encoding.UTF8.GetBytes(message);

            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            Console.WriteLine("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));

            // Используем рекурсию для неоднократного вызова SendMessageFromSocket()
            if (message.IndexOf("end") == -1) SendMessageFromSocket(domain, port);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
