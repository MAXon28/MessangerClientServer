using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ChatLibrary;
using Server.BusinessLogic;
using Server.Human;
using Server.Interface;

namespace Server.Network
{
    class Client
    {
        private Socket _clientSocket;
        private ServerObject _server;
        private UserService _userService;
        private ChatService _chatService;
        private IHuman _human;

        public Client(Socket clientSocket, ServerObject server, UserService userService, Guid id, string gender, string name)
        {
            _clientSocket = clientSocket;
            _server = server;
            _userService = userService;
            _chatService = new ChatService(userService.GetUnitOfWork());
            Id = id;
            if (gender == "woman")
            {
                _human = new Woman();
            }
            else
            {
                _human = new Man();
            }
            Name = name;
            var networkStream = new NetworkStream(_clientSocket);
            Writer = new BinaryWriter(networkStream);
            Reader = new BinaryReader(networkStream);
            IsCanNextMessage = true;
            UserActivePage = "Chat page";
            _server.AddConnection(this);
        }

        public Guid Id { get; }

        public string Name { get; private set; }

        public BinaryWriter Writer { get; }

        public BinaryReader Reader { get; }

        public bool IsCanNextMessage { get; private set; }

        public string UserActivePage { get; private set; }

        public ChatMessageDTO PastMessage { get; set; }

        public void Process()
        {
            try
            {
                string message = GetNotification("Entry");
                _server.BroadcastMessageAboutEnteringUser(message, Id, "29");

                while (true)
                {
                    try
                    {
                        switch (GetRequestCode())
                        {
                            case 2:
                                GetNotification();
                                SendMessage();
                                break;
                            case 3:
                                GetMessages();
                                break;
                            case 4:
                                string newName = Reader.ReadString();
                                if (_userService.IsUniqueData(name: newName))
                                {
                                    Name = newName;
                                    _userService.UpdateUserAsync(1, Id, newName);
                                    Writer.Write("40");
                                    Writer.Write(Name);
                                }
                                else
                                {
                                    Writer.Write("41");
                                }
                                break;
                            case 9:
                                UserActivePage = UserActivePage == "Chat page" ? "No Chat page" : "Chat page";
                                Writer.Write("");
                                Writer.Flush();
                                break;
                            case 12:
                                IsCanNextMessage = Reader.ReadBoolean();
                                break;
                            default:
                                break;
                        }
                    }
                    catch
                    {
                        message = GetNotification("Exit");
                        _server.BroadcastMessageAboutEnteringUser(message, Id, "29");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                _server.RemoveConnection(Id);
                Close();
            }
        }

        private int GetRequestCode()
        {
            return Reader.ReadInt32();
        }

        private string GetNotification(string typeNotifications)
        {
            switch (typeNotifications)
            {
                case "Entry":
                    return _human.EntryInToTheChat(Name);
                case "Exit":
                    return _human.OutOfTheChat(Name);
                default:
                    return null;
            }
        }

        private void GetNotification()
        {
            _human.SendMessage(Name);
        }

        private void SendMessage()
        {
            IsCanNextMessage = true;
            string message = Reader.ReadString();
            DateTime timeSendMessage = DateTime.Now; 
            _server.BroadcastMessageFromUser("30", Name, message, timeSendMessage.ToString(), Id);
            PastMessage = _chatService.AddMessage(Id, timeSendMessage, message, Name);
        }

        private void GetMessages()
        {
            IFormatter formatter = new BinaryFormatter();
            NetworkStream strm = new NetworkStream(_clientSocket);
            List<ChatMessageDTO> list = new List<ChatMessageDTO>();
            string typeRequest = Reader.ReadString();
            if (typeRequest == "Have messages")
            { 
                var messages = (List<ChatMessageDTO>) formatter.Deserialize(strm);
                (List<ChatMessageDTO>, int) data = _chatService.GetNewMessagesAndAllCountNotReadMessages(messages);
                list = data.Item1;
                PastMessage = list.Count > 0 ? list[list.Count - 1] : messages[messages.Count - 1];
                Writer.Write(data.Item2);
                Writer.Flush();
            }
            else if(typeRequest == "No messages")
            {
                list = _chatService.GetChatMessages();
                if (!IsCanNextMessage)
                {
                    Writer.Write("31");
                    Writer.Flush();
                    IsCanNextMessage = true;
                    return;
                }
            }
            else if(typeRequest == "Actual messages")
            {
                list = _chatService.GetChatMessages();
                Writer.Write("67");
                Writer.Write(0);
                Writer.Flush();
                PastMessage = list.Count > 0 ? list[list.Count - 1] : PastMessage;
            }
            else if(typeRequest == "New messages")
            {
                if (PastMessage?.Id == Reader.ReadInt32())
                {
                    (List<ChatMessageDTO>, int) data = _chatService.GetNewMessagesAndAllCountNotReadMessages(PastMessage);
                    list = data.Item1;
                    PastMessage = list.Count > 0 ? list[list.Count - 1] : PastMessage;
                    IsCanNextMessage = list.Count < 4;
                    Writer.Write("67");
                    Writer.Write(data.Item2);
                    Writer.Flush();
                }
                else
                {
                    return;
                }
            }
            else if (typeRequest == "New-new messages")
            {
                var message = (ChatMessageDTO)formatter.Deserialize(strm);
                (List<ChatMessageDTO>, int) data = _chatService.GetNewMessagesAndAllCountNotReadMessages(message);
                list = data.Item1;
                PastMessage = list.Count > 0 ? list[list.Count - 1] : message;
                IsCanNextMessage = data.Item2 == 0;
                Writer.Write("67");
                Writer.Write(data.Item2);
                Writer.Flush();
            }
            else
            {
                list = _chatService.GetChatMessages(Reader.ReadInt32());
            }

            if (typeRequest == "Have ID")
            {
                Writer.Write("31");
                Writer.Flush();
            }

            formatter.Serialize(strm, list);
            strm.Close();
        }

        public void Close()
        {
            Writer?.Close();
            Reader?.Close();
            _clientSocket?.Close();
        }
    }
}