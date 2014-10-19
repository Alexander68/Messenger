using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Messenger.ui;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using MessageException;
using Messenger.utils;

namespace Messenger
{
    public class SocketServer
    {
        private string domain;
        private int serverPort;
       // private Socket handler;
        private Socket sListener;
        private IPEndPoint ipEndPoint;
        public MainWindow mainWindow;


        public SocketServer(string domain, int port)
        {
            this.domain = domain;
            this.serverPort = port;
            char[] privateKey = CriptoUtils.Generic(32);
            Properties.setPrivateKey(new string(privateKey));
        }

        public void setWindow(MainWindow window)
        {
            this.mainWindow = window;
        }


        private void initSocket()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(domain);
            IPAddress ipAddr;
            if (ipHost.AddressList.Length != 0) ipAddr = ipHost.AddressList[0];
            else throw new IpNotFoundExecption("Ip's not found");
            ipEndPoint = new IPEndPoint(ipAddr, serverPort);
            sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sListener.Bind(ipEndPoint);
            
        }



        private bool isCommunication()
        {
            DialogResult result = MessageBox.Show("Хотите начать общение?",
                                                  "Установка соединения",
                                                  MessageBoxButtons.YesNo);
            return DialogResult.Yes.Equals(result) ? true : false;
        }

        private void BeginCommunication(Socket handler)
        {
            try
            {
                bool isComm = isCommunication();
                if (!isComm)
                {
                    handler.Send(Encoding.UTF8.GetBytes(Properties.NO_COMMUNICATION));
                    return;
                }


                string privateKey = Properties.getPrivateKey();
                PublicKey publicKey = Properties.getPublicKey();

                char[] sendKey = CriptoUtils.createSesionKey(publicKey, privateKey.ToCharArray());
                handler.Send(Encoding.UTF8.GetBytes(sendKey));

                string recvKey = SocketUtils.RecvData(handler);
                Console.WriteLine(recvKey.Length + " " + privateKey.Length);
                char[] sessionKey = CriptoUtils.createSesionKey(new PublicKey(recvKey, publicKey.mod), privateKey.ToCharArray());
                Properties.setSessionKey(new string(sessionKey));
                string encodedString = CriptoUtils.Encrypt(Properties.TEST_COMMUNICATON, Properties.getSessionKey());
                handler.Send(Encoding.UTF8.GetBytes(encodedString));
                mainWindow.write(SocketClient.SUCCESS);
                mainWindow.setEnabledSend();
            }
            catch (Exception e)
            {
                mainWindow.write("Не удалось установить соединение по причине: " + e.Message);
            }
        }


        public void Init()
        {
            initSocket();

            sListener.Listen(10);
            while (true)
            {
                Socket handler = sListener.Accept();
                string data = null;
                try
                {
                    data += SocketUtils.RecvData(handler);
                    Console.WriteLine("Начал приёмку от {0} данные {1}", handler.RemoteEndPoint.ToString(), data);
                    switch (data)
                    {
                        case Properties.BEGIN:
                            {
                                BeginCommunication(handler);
                                break;
                            }

                        default:
                            {
                                mainWindow.write(CriptoUtils.Decrypt(data, Properties.getSessionKey()));
                                break;
                            }
                    }
                    close(handler);
                   
                }
                catch (Exception e)
                {
                    mainWindow.write(e.Message);
                }
            }
        }


        public void close(Socket handler)
        {
            if (handler != null)
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
    }
}
