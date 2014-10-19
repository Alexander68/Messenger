using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using Messenger.utils;

namespace Messenger
{
    public class SocketUtils
    {
         

        public static string RecvData(Socket handler)
        {
            byte[] bigBuffer = new byte[1020004];
            int bytesRec = handler.Receive(bigBuffer);
            return Encoding.UTF8.GetString(bigBuffer, 0, bytesRec);             
        }
    }
}
