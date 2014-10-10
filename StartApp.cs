using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Messenger
{
    class StartApp
    {
        public static void Main(String[] args)
        {
            SocketClient client = new SocketClient("localhost", 9999);
            SocketServer server = new SocketServer("localhost", 9999);

            Thread clientThr = new Thread(new ThreadStart(client.init));
            Thread serverThr = new Thread(new ThreadStart(server.init));
            clientThr.Start();
            serverThr.Start();
        }
    }
}
