using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Server.Network;

namespace Server
{
    class Program
    {
        static ServerObject _server;
        static Thread _listenThread;

        static void Main(string[] args)
        {
            Console.Title = "Server";
            Console.ForegroundColor = ConsoleColor.Green;

            try
            {
                _server = new ServerObject();
                _listenThread = new Thread(new ThreadStart(_server.Listen));
                _listenThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                _server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}