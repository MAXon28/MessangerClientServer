using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        //private const int PORT = 8080;

        static void Main(string[] args)
        {
            const int PORT = 8080;

            var tcpEndPoint = new IPEndPoint(IPAddress.Any, PORT);

            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Bind(tcpEndPoint);
            tcpSocket.Listen(2812);

            while (true)
            {
                Socket listener = tcpSocket.Accept();
                var bufferBytes = new byte[256];
                var size = 0;
                var data = new StringBuilder();

                do
                {
                    size = listener.Receive(bufferBytes);
                    data.Append(Encoding.UTF8.GetString(bufferBytes, 0, size));
                } 
                while (listener.Available > 0);

                Console.WriteLine(data);

                //listener.Send(Encoding.UTF8.GetBytes("Прошло!"));

                listener.Shutdown(SocketShutdown.Both);
                listener.Close();
            }

        }
    }
}

/*<StackPanel DockPanel.Dock="Top" Orientation= "Horizontal" >
< TextBox
Height= "116"
TextWrapping= "Wrap"
Text= "{Binding Text}"
Width= "728" />
< Button
Command= "{Binding Send}"
Content= "Отправить"
Width= "164" />
</ StackPanel >*/