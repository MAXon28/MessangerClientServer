using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using ChatClient.Interface;
using ChatClient.Model;
using ChatLibrary;

namespace ChatClient.Logic
{
    delegate void DelegateReceiveMessage(BinaryReader binaryReader);

    class ServerWorker
    {
        private static ServerWorker _serverWorker;
        private Socket _client;
        private BinaryWriter _writer;
        private DelegateReceiveMessage _delegateReceiveMessage;
        private ServerConnection _serverConnection;
        private Thread _thread;

        private ServerWorker()
        {
            _serverConnection = ServerConnection.NewInstance();
        }

        public static ServerWorker NewInstance()
        {
            if (_serverWorker == null)
            {
                _serverWorker = new ServerWorker();
            }
            return _serverWorker;
        }

        public int Registration(string login, string password, string gender, string name)
        {
            IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(_serverConnection.Ip), _serverConnection.Port);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(tcpEndPoint);

            NetworkStream networkStreamToWrite = new NetworkStream(client);
            BinaryWriter writer = new BinaryWriter(networkStreamToWrite);

            writer.Write(0);
            writer.Write(login);
            writer.Write(password);
            writer.Write(gender);
            writer.Write(name);

            writer.Flush();

            var bufferBytes = new byte[2];
            var answer = new StringBuilder();
            do
            {
                int size = client.Receive(bufferBytes);
                answer.Append(Encoding.UTF8.GetString(bufferBytes, 0, size));
            }
            while (client.Available > 0);

            client.Shutdown(SocketShutdown.Both);
            client.Close();

            return int.Parse(answer.ToString());
        }

        public string Authorization(string login, string password, ref string name)
        {
            IPEndPoint tcpEndPoint = new IPEndPoint(IPAddress.Parse(_serverConnection.Ip), _serverConnection.Port);
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _client.Connect(tcpEndPoint);

            var networkStreamToWrite = new NetworkStream(_client);
            var writer = new BinaryWriter(networkStreamToWrite);

            writer.Write(1);
            writer.Write(login);
            writer.Write(password);

            writer.Flush();

            var networkStreamReader = new NetworkStream(_client);
            var reader = new BinaryReader(networkStreamReader);

            if (reader.ReadString() == "28")
            {
                name = reader.ReadString();
                _thread = new Thread(ReceiveMessage) { IsBackground = true };
                _thread.Start();
                return "28";
            }

            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
            return "34";
        }

        public (List<ChatMessageDTO>, int) GetMessagesStart(List<ChatMessageDTO> messages)
        {
            var networkStream = new NetworkStream(_client);
            var writer = new BinaryWriter(networkStream);

            writer.Write(3);
            writer.Write("Have messages");
            writer.Flush();

            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(networkStream, messages);

            var reader = new BinaryReader(networkStream);
            int count = reader.ReadInt32();

            var list = (List<ChatMessageDTO>)formatter.Deserialize(networkStream);
            return (list, count);
        }

        public List<ChatMessageDTO> GetMessagesStart()
        {
            var networkStream = new NetworkStream(_client);
            var writer = new BinaryWriter(networkStream);

            writer.Write(3);
            writer.Write("No messages");
            writer.Flush();

            IFormatter formatter = new BinaryFormatter();
            var list = (List<ChatMessageDTO>)formatter.Deserialize(networkStream);
            return list;
        }

        public void GetMessagesActual()
        {
            NetworkStream = new NetworkStream(_client);
            var writer = new BinaryWriter(NetworkStream);

            writer.Write(3);
            writer.Write("Actual messages");
            writer.Flush();
        }

        public void GetMessages(int id, string typeLoad)
        {
            NetworkStream = new NetworkStream(_client);
            var writer = new BinaryWriter(NetworkStream);

            writer.Write(3);
            writer.Write(typeLoad);
            writer.Write(id);
            writer.Flush();
        }

        public void GetMessages(ChatMessageDTO message)
        {
            NetworkStream = new NetworkStream(_client);
            var writer = new BinaryWriter(NetworkStream);

            writer.Write(3);
            writer.Write("New-new messages");
            writer.Flush();

            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(NetworkStream, message);
        }

        public void SendMessage(string message)
        {
            var networkStreamToWrite = new NetworkStream(_client);
            var writer = new BinaryWriter(networkStreamToWrite);

            writer.Write(2);
            writer.Write(message);
            writer.Flush();
        }

        public void GetUpdate(bool isCanViewNewMessage)
        {
            var networkStreamToWrite = new NetworkStream(_client);
            var writer = new BinaryWriter(networkStreamToWrite);

            writer.Write(12);
            writer.Write(isCanViewNewMessage);
            writer.Flush();
        }

        private void ReceiveMessage()
        {
            while (true)
            {
                Thread.Sleep(1);
                try
                {
                    var networkStreamReader = new NetworkStream(_client);
                    var reader = new BinaryReader(networkStreamReader);
                    _delegateReceiveMessage?.Invoke(reader);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void RewriteDelegate(IViewModel viewModel)
        {
            _delegateReceiveMessage = viewModel.Notification;
        }

        public NetworkStream NetworkStream { get; private set; }
    }
}