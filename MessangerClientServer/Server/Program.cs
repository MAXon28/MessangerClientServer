using System;
using System.Threading;
using Server.Network;

namespace Server
{
    class Program
    {
        static ServerObject server;
        static Thread listenThread;

        static void Main(string[] args)
        {
            Console.Title = "Server";
            Console.ForegroundColor = ConsoleColor.Green;

            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
    }
}