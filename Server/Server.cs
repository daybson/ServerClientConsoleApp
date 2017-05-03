using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        private TcpListener server;
        private bool isRunning;

        public Server(int port)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isRunning = true;
            LoopClients();
        }

        private void LoopClients()
        {
            while(isRunning)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread t = new Thread(new ParameterizedThreadStart(HandleCliend));
                t.Start(client);
            }
        }

        private void HandleCliend(object obj)
        {
            TcpClient client = (TcpClient) obj;
            StreamWriter w = new StreamWriter(client.GetStream(), Encoding.UTF8);
            StreamReader r = new StreamReader(client.GetStream(), Encoding.UTF8);

            bool clientConnected = true;
            string data = null;

            while(clientConnected)
            {
                data = r.ReadLine();
                Console.WriteLine($"Client {client.Client.RemoteEndPoint.ToString()}: {data}");
            }
        }
    }
}
