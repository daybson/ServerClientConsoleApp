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
        private Thread threadReading;

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
            while (isRunning)
            {
                TcpClient tcpClient = server.AcceptTcpClient();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"Novo cliente conectado: {tcpClient.Client.RemoteEndPoint.ToString()}");

                this.threadReading = new Thread(new ParameterizedThreadStart(ReadData));
                //Thread writer = new Thread(new ParameterizedThreadStart(WriteData));
                var stream = tcpClient.GetStream();
                var client = new ClientData(clients.Count, tcpClient, stream);

                lock (clients)
                {
                    clients.Add(clients.Count, client);
                }

                this.threadReading.Start(client);
            }
        }


        private void ReadData(object obj)
        {
            bool clientConnected = true;
            ClientData clientData = (ClientData)obj;

            while (clientConnected)
            {
                string data = null;
                byte[] buff = null;
                buff = new byte[1024];
                int count = 0;

                try
                {
                    count = clientData.stream.Read(buff, 0, buff.Length);
                    if (count > 0)
                    {
                        data = Encoding.UTF8.GetString(buff);
                        data = data.Substring(0, data.IndexOf("\0"));

                        Console.ForegroundColor = ConsoleColor.DarkBlue;

                        lock (clients)
                        {
                            foreach (var cl in clients)
                            {
                                Console.WriteLine($"Replicando mensagem para {cl.Value.tcpClient.Client.RemoteEndPoint.ToString()}: {data}");
                                cl.Value.stream.Write(buff, 0, buff.Length);
                                cl.Value.stream.Flush();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    lock (this.clients)
                    {
                        this.clients.Remove(clientData.id);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Cliente {clientData.tcpClient.Client.RemoteEndPoint.ToString()} " +
                                          $"está offline.\n" +
                                          $"Clientes conectados: {this.clients.Count}");
                        clientData.tcpClient.Client.Shutdown(SocketShutdown.Both);
                        clientData.tcpClient.Client.Close();
                        this.threadReading.Abort();
                    }
                    break;
                }
            }
        }


        private class ClientData
        {
            public int id { get; private set; }
            public TcpClient tcpClient { get; private set; }
            public NetworkStream stream { get; private set; }

            public ClientData(int id, TcpClient tcpClient, NetworkStream stream)
            {
                this.id = id;
                this.tcpClient = tcpClient;
                this.stream = stream;
            }
        }
    }
}
