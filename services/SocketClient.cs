using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MessageException;
using Messenger.utils;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Messenger
{
    public class StateObject
    {
        //public Socket workSocket = null;
        public const int BUFFER_SIZE = 1024;
        public byte[] buffer = new byte[BUFFER_SIZE];
        public StringBuilder sb = new StringBuilder();
        public int status = 0;

       
    }

    public class SocketClient
    {
        public const string SUCCESS = "Соединение установленно";
        public const string FAILED = "Соединение не установленно";
        public const string REJECT = "Собеседник отказался устанавливать соединение";
        private string ip;
        private int port;

        public SocketClient(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        private Socket init()
        {
            try
            {
                IPHostEntry ipHost = Dns.GetHostEntry(ip);
                IPAddress ipAddr;

                if (ipHost.AddressList.Length != 0) ipAddr = ipHost.AddressList[0];
                else throw new IpNotFoundExecption("Ip's not found");

                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
                Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                sender.Connect(ipEndPoint);
                return sender;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string connect()
        {
            Socket sender = null;
            try
            {
                sender = init();
                sender.Send(Encoding.UTF8.GetBytes(Properties.BEGIN));
                string recvKey = SocketUtils.RecvData(sender);
                Console.WriteLine(recvKey);
                if (Properties.NO_COMMUNICATION.Equals(recvKey)) return REJECT;

                string privateKey = Properties.getPrivateKey();
                PublicKey publicKey = Properties.getPublicKey();
                char[] sendKey = CriptoUtils.createSesionKey(publicKey, privateKey.ToCharArray());
                sender.Send(Encoding.UTF8.GetBytes(sendKey));

                Console.WriteLine(recvKey.Length + " " + privateKey.Length);
                char[] sessionKey = CriptoUtils.createSesionKey(new PublicKey(recvKey, publicKey.mod), privateKey.ToCharArray());
                Properties.setSessionKey(new string(sessionKey));

                string testMsg = CriptoUtils.Decrypt(SocketUtils.RecvData(sender), Properties.getSessionKey());

                return Properties.TEST_COMMUNICATON.Equals(testMsg) ? SUCCESS : FAILED;
/*
                //so2.workSocket = sender;
                //Socket temp = sender;
                //sender.BeginReceive(so2.buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(Read_Callback), so2);	
                //sender.BeginReceive(recvBuf, 0, recvBuf.Length, 0, recBack, null);
                //so2.workSocket.Close();
                Stopwatch stopWatch = new Stopwatch();
                TimeSpan ts = new TimeSpan();
                stopWatch.Start();
                int bytesRec = 0;
                //Thread.Sleep(3000);
                do
                {
                    //Thread.Sleep(3000);
                    ts = stopWatch.Elapsed;
                    //Console.WriteLine(connec);
                } while ((ts.Seconds < (int)2) && connec!=true);
                //close();
                //sender = null;
                //init();
                stopWatch.Stop();

                return "fail";
                */
                
            }
            catch (Exception e)
            {   
                return e.Message;
            }
            finally
            {
                close(sender);
            }
        }



        public bool SendMessageFromSocket( string message, bool isCrypto)
        {
            Socket sender = null;
            try
            {
                sender = init();
                Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());
                byte[] msg = Encoding.UTF8.GetBytes(isCrypto? CriptoUtils.Encrypt(message, Properties.getSessionKey()): message);
                int bytesSent = sender.Send(msg);
                Console.WriteLine("Сокет послал с {0} ", sender.RemoteEndPoint.ToString());
                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                close(sender);
            }
        }


        public void close(Socket sender)
        {
            if (sender != null)
            {
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
        }


/*

       public static void Read_Callback(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            //Socket s = so.workSocket;

            int read = sender.EndReceive(ar);

            if (read > 0)
            {
                so.sb.Append(Encoding.ASCII.GetString(so.buffer, 0, read));
                sender.BeginReceive(so.buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(Read_Callback), so);
            }
            else
            {
                if (so.sb.Length > 1)
                {
                    //All of the data has been read, so displays it to the console
                    string strContent;
                    strContent = so.sb.ToString();
                    Console.WriteLine(String.Format("Read {0} byte from socket" +
                                     "data = {1} ", strContent.Length, strContent));
                    connec = true;
                    
                }
                //s = null;
                //s.Shutdown(SocketShutdown.Both);
                //s.Close();
                //so.workSocket.Shutdown(SocketShutdown.Both);
                //so.workSocket.Close();

                
            }
        }



        */

       

    
    }
}
