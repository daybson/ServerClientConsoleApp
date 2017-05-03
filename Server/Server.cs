using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    public class Server
    {
        private TcpListener server;
        private bool isRunning;
        private Dictionary<int, ClientData> clients;

        public Server(int port)
        {
            clients = new Dictionary<int, ClientData>();
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isRunning = true;
            LoopClients();
        }

        private void LoopClients()
        {
            while(isRunning)
            {
                TcpClient tcpClient = server.AcceptTcpClient();
                Console.WriteLine($"Novo cliente conectado: {tcpClient.Client.RemoteEndPoint.ToString()}");

                Thread reader = new Thread(new ParameterizedThreadStart(ReadData));
                Thread writer = new Thread(new ParameterizedThreadStart(WriteData));
                var stream = tcpClient.GetStream();
                var client = new ClientData(clients.Count, tcpClient, stream);

                lock(clients)
                {
                    clients.Add(clients.Count, client);
                }

                reader.Start(client);
                writer.Start(client);
                //Thread t = new Thread(new ParameterizedThreadStart(HandleCliend));
                //t.Start(client);
            }
        }


        private void ReadData(object obj)
        {
            var client = (ClientData) obj;
            var clientConnected = true;
            string data = null;

            while(clientConnected)
            {
                if(!client.stream.DataAvailable)
                    continue;

                var buff = new byte[2048];

                lock(clients)
                {
                    int count = client.stream.Read(buff, 0, 2048);

                    if(count > 0)
                    {
                        data = Encoding.UTF8.GetString(buff);
                        data = data.Substring(0, data.IndexOf("\0"));
                        var c = clients.ToList().Find(x => x.Value.id == client.id).Value;
                        c.LastData = data;
                        Console.WriteLine($"Client {client.tcpClient.Client.RemoteEndPoint.ToString()}: {data}");
                        c.LastData = "";
                    }
                }
            }
        }


        private void WriteData(object obj)
        {
            var client = (ClientData) obj;
            var clientConnected = true;

            while(clientConnected)
            {
                lock(clients)
                {
                    foreach(var c in clients)
                    {
                        if(c.Value.LastData?.Length > 0)
                        {
                            var buff = Encoding.UTF8.GetBytes($"Client: {c.Value.LastData}");
                            client.stream.Write(buff, 0, buff.Length);
                            client.stream.Flush();
                        }
                    }
                }
            }
        }


        private void HandleCliend(object obj)
        {
            /*
            var tcpClient = (TcpClient) obj;
            var w = new StreamWriter(tcpClient.GetStream(), Encoding.UTF8);
            var r = new StreamReader(tcpClient.GetStream(), Encoding.UTF8);
            var c = new ClientData(clients.Count, tcpClient, w, r);

            lock(clients)
            {
                clients.Add(clients.Count, c);
            }

            var clientConnected = true;
            string data = null;

            while(clientConnected)
            {
                lock(clients)
                {
                    //foreach(var c in clients)//.Where(x => x.Value.id != clientData.id))
                    {
                        data = c.r.ReadLine();
                        Console.WriteLine($"Client {c.tcpClient.Client.RemoteEndPoint.ToString()}: {data}");

                        c.w.Write($"Client {c.tcpClient.Client.RemoteEndPoint.ToString()}: {data}");
                        c.w.Flush();
                    }
                }
            }
            */
        }

        private class ClientData
        {
            public int id { get; private set; }
            public TcpClient tcpClient { get; private set; }
            public NetworkStream stream { get; private set; }
            public string LastData;

            public ClientData(int id, TcpClient tcpClient, NetworkStream stream)
            {
                this.id = id;
                this.tcpClient = tcpClient;
                this.stream = stream;

            }
        }
    }
}
