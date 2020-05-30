using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        public MainViewModel() { }

        public string Login { get; set; }

        public string Password { get; set; }

        public bool Remember { get; set; }

        public ICommand Send
        {
            get
            {
                return new RelayCommand(() => { ToServer(); });
            }
        }

        private void ToServer()
        {
            string ip = "127.0.0.1";
            int port = 8080;
            IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //var data = Encoding.UTF8.GetBytes(Text);

            client.Connect(tcpEndPoint);

            //client.Send(data);

            //BinaryReader reader = new BinaryReader(client.St);

            /*var bufferBytes = new byte[256];
            var size = 0;
            var answer = new StringBuilder();

            do
            {
                size = client.Receive(bufferBytes);
                answer.Append(Encoding.UTF8.GetString(bufferBytes, 0, size));
            }
            while (client.Available > 0);

            Text = answer.ToString();*/

            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}