using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ChatLibrary;
using Server.BusinessLogic;
using Server.DataTransferObject;
using Server.Human;
using Server.Interface;

namespace Server.Network
{
    class ServerObject
    {
        private Socket _tcpSocket;
        private List<Client> _clients;
        private UserService _userService;

        public ServerObject()
        {
            _clients = new List<Client>();
            _userService = new UserService();
        }

        internal void Listen()
        {
            try
            {
                IPAddress[] IPs = Dns.GetHostAddresses(Dns.GetHostName());
                Console.WriteLine($@"IP-адрес данного сервера: {IPs[IPs.Length - 1]}");

                int port = -1;
                bool isCanContinue = false;
                while (isCanContinue == false)
                {
                    Console.Write("Введите номер порта: ");
                    try
                    {
                        port = int.Parse(Console.ReadLine());
                        isCanContinue = true;
                    }
                    catch
                    {
                        Console.WriteLine("Такого порта не существует! Повторите попытку!");
                    }
                }

                var tcpEndPoint = new IPEndPoint(IPAddress.Any, port);
                _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _tcpSocket.Bind(tcpEndPoint);
                _tcpSocket.Listen(2812);
                Console.WriteLine("Server is running. Waiting connections...");

                while (true)
                {
                    Socket tcpClient = _tcpSocket.Accept();

                    var networkStreamReader = new NetworkStream(tcpClient);
                    var reader = new BinaryReader(networkStreamReader);
                    if (reader.ReadInt32() == 0)
                    {
                        SendResultCode(tcpClient, Registration(reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadString()));

                        tcpClient.Shutdown(SocketShutdown.Both);
                        tcpClient.Close();
                    }
                    else
                    {
                        Guid userId = new Guid();
                        (string, UserDTO) resultAuth = Authorization(reader.ReadString(), reader.ReadString(), ref userId);

                        var networkStreamWriter = new NetworkStream(tcpClient);
                        var writer = new BinaryWriter(networkStreamWriter);
                        writer.Write(resultAuth.Item1);

                        if (resultAuth.Item1 == "28")
                        {
                            Console.ForegroundColor = resultAuth.Item2.Gender == "woman" ? new Woman().GetForeground() : new Man().GetForeground();
                            Console.Write(resultAuth.Item2.Name);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(" онлайн!");

                            writer.Write(resultAuth.Item2.Name);

                            Client client = new Client(tcpClient, this, _userService, userId, resultAuth.Item2.Gender, resultAuth.Item2.Name);
                            Thread threadClient = new Thread(client.Process);
                            threadClient.Start();
                        }

                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        private void SendResultCode(Socket client, string resultCode)
        {
            var data = new byte[2];
            data = Encoding.UTF8.GetBytes(resultCode);
            client.Send(data);
        }

        private string Registration(string login, string password, string gender, string name)
        {
            if (_userService.IsUniqueData(login, name))
            {
                _userService.AddUser(login, password, gender, name);
                return "27";
            }

            return "33";
        }

        private (string, UserDTO) Authorization(string login, string password, ref Guid userId)
        {
            UserDTO user = _userService.ValidationData(login, password, ref userId);

            if (user != null)
            {
                return ("28", user);
            }

            return ("34", null);
        }

        public void AddConnection(Client client)
        {
            _clients.Add(client);
        }

        public void BroadcastMessageAboutEnteringUser(string message, Guid userGuidWhoEnterAtChat, string code)
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                if (_clients[i].Id != userGuidWhoEnterAtChat)
                {
                    _clients[i].Writer.Write(code);
                    _clients[i].Writer.Write(message);
                    _clients[i].Writer.Flush();
                }
            }
        }

        public void BroadcastMessageFromUser(string code, string name, string message, string date, Guid userGuidWhoSendMessage)
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                if (_clients[i].Id != userGuidWhoSendMessage)
                {
                    if (_clients[i].UserActivePage == "Chat page")
                    {
                        if (_clients[i].IsCanNextMessage)
                        {
                            _clients[i].Writer.Write(code);
                            _clients[i].Writer.Write(name);
                            _clients[i].Writer.Write(message);
                            _clients[i].Writer.Write(date);
                            _clients[i].Writer.Flush();
                        }
                        else
                        {
                            _clients[i].Writer.Write(code);
                            _clients[i].Writer.Write("+1");
                            _clients[i].Writer.Flush();
                        }
                        _clients[i].PastMessage = new ChatMessageDTO
                        {
                            Id = _clients[i].PastMessage.Id + 1,
                            SenderName = name,
                            SendMessage = message,
                            DateSend = date
                        };
                    }
                    else
                    {
                        _clients[i].Writer.Write(code);
                        _clients[i].Writer.Write(name);
                        _clients[i].Writer.Flush();
                    }
                }
            }
        }

        public void RemoveConnection(Guid idUserWhoMustBeDelete)
        {
            Client client = _clients.FirstOrDefault(c => c.Id == idUserWhoMustBeDelete);
            if (client != null)
            {
                _clients.Remove(client);
            }
        }

        public void Disconnect()
        {
            _tcpSocket.Close(); //остановка сервера

            for (int i = 0; i < _clients.Count; i++)
            {
                _clients[i].Close();
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}