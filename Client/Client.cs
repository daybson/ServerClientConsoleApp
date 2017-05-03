using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
        private TcpClient client;
        private StreamWriter w;
        private StreamReader r;
        private bool isConnected;

        public Client(string ipAddress, int port)
        {
            client = new TcpClient();
            client.Connect(ipAddress, port);

            HandleCommunication();
        }

        private void HandleCommunication()
        {
            r = new StreamReader(client.GetStream(), Encoding.UTF8);
            w = new StreamWriter(client.GetStream(), Encoding.UTF8);

            isConnected = true;
            string data = null;

            while(isConnected)
            {
                Console.WriteLine("Type: ");
                data = Console.ReadLine();
                w.WriteLine(data);
                w.Flush();
            }
        }
    }
}
