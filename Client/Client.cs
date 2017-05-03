using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

            Thread reader = new Thread(ReadData);
            Thread writer = new Thread(WriteData);

            isConnected = true;

            reader.Start();
            writer.Start();

            //HandleCommunication();
        }


        private void ReadData()
        {
            r = new StreamReader(client.GetStream(), Encoding.UTF8);
            while(isConnected)
            {
                string data = r.ReadLine();
                Console.WriteLine($"Message received: {data}");
            }
        }


        private void WriteData()
        {
            w = new StreamWriter(client.GetStream(), Encoding.UTF8);
            string data = null;
            while(isConnected)
            {
                Console.Write("Type a message: ");
                data = Console.ReadLine();
                w.WriteLine(data);
                w.Flush();
            }
        }


        private void HandleCommunication()
        {
            r = new StreamReader(client.GetStream(), Encoding.UTF8);
            w = new StreamWriter(client.GetStream(), Encoding.UTF8);

            isConnected = true;
            string data = null;

            while(isConnected)
            {
                Console.Write("Type a message: ");
                data = Console.ReadLine();
                w.WriteLine(data);
                w.Flush();

                Console.WriteLine($"Message received: {r.ReadLine()}");
            }
        }
    }
}
