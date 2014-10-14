using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Messenger.ui;
using System.Windows.Forms;
using MessageEception;

namespace Messenger
{
    public class SocketServer
    {
        private String domain;
        private int port;
        public MainWindow mainWindow;

        public SocketServer(String domain, int port)
        {
            this.domain = domain;
            this.port = port;
        }


        public void setWindow(MainWindow window) 
        {
            this.mainWindow = window;
        }

        public void init()
        {             
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

            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);
                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    string data = null;

                    // Мы дождались клиента, пытающегося с нами соединиться

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);


                    string reply = "Спасибо за запрос в " + data
                            + " символов"; 
                        
                        mainWindow.write(data);

                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    if (data.IndexOf("end") > -1)
                    {
                        Console.WriteLine("Сервер завершил соединение с клиентом.");
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
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
    }
}
