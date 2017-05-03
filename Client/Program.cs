using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Client IP:");
            string ip = Console.ReadLine();

            Console.Write("Port:");
            int port = Int32.Parse(Console.ReadLine());

            Client c = new Client(ip, port);
        }
    }
}
