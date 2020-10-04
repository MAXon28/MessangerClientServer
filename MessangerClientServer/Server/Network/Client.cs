using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ChatLibrary;
using ChatLibrary.DataTransferObject;
using Server.BusinessLogic;
using Server.Game;
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
        private SettingsService _settingsService;
        private GameLogic _gameLogic;
        private IHuman _human;

        private const int WRITE_MESSAGE = 2;
        private const int LOAD_MESSAGES = 3;
        private const int REWRITE_USER_NAME = 4;
        private const int REWRITE_USER_GENDER = 5;
        private const int REWRITE_USER_LOGIN = 6;
        private const int REWRITE_USER_PASSWORD = 7;
        private const int EXIT = 8;
        private const int NOTIFICATION_ABOUT_GO_TO_NEW_PAGE = 9;
        private const int LOAD_USERS = 10;
        private const int PLAY_GAME = 11;
        private const int NOTIFICATION_ABOUT_USER_CAN_ACCEPT_NEW_MESSAGE = 12;
        private const int PlLAYER_S_MOVE = 110;
        private const int OUT_THE_GAME = 111;

        public Client(Socket clientSocket, ServerObject server, UserService userService, Guid id, string gender, string name)
        {
            _clientSocket = clientSocket;
            _server = server;
            _userService = userService;
            _chatService = new ChatService(userService.GetUnitOfWork());
            _settingsService = new SettingsService(_chatService.GetUnitOfWork());
            Id = id;
            SetGender(gender);
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
                            case WRITE_MESSAGE:
                                GetNotification();
                                SendMessage();
                                break;
                            case LOAD_MESSAGES:
                                GetMessages();
                                break;
                            case REWRITE_USER_NAME:
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
                            case REWRITE_USER_GENDER:
                                string newGender = Reader.ReadString();
                                SetGender(newGender);
                                _userService.UpdateUserAsync(2, Id, newGender);
                                break;
                            case REWRITE_USER_LOGIN:
                                string newLogin = Reader.ReadString();
                                if (_userService.IsUniqueData(login: newLogin))
                                {
                                    _userService.UpdateUserAsync(3, Id, newLogin);
                                    Writer.Write("42");
                                    Writer.Write(newLogin);
                                }
                                else
                                {
                                    Writer.Write("43");
                                }
                                break;
                            case REWRITE_USER_PASSWORD:
                                _userService.UpdateUserAsync(4, Id, Reader.ReadString());
                                break;
                            case EXIT:
                                _server.RemoveConnection(Id);
                                Close();
                                break;
                            case NOTIFICATION_ABOUT_GO_TO_NEW_PAGE:
                                UserActivePage = Reader.ReadString();
                                Writer.Write("");
                                Writer.Flush();
                                break;
                            case LOAD_USERS:
                                List<UserDTO> users = _userService.GetUsers();
                                List<Client> clients = _server.Clients;
                                foreach (var client in clients)
                                {
                                    List<UserDTO> usersList = (from u in users
                                        where u.Name == client.Name
                                        select u).ToList();
                                    if (usersList.Count == 1)
                                    {
                                        int indexUser = users.IndexOf(usersList[0]);
                                        users[indexUser].PastOnline = null;
                                    }
                                }
                                IFormatter formatter = new BinaryFormatter();
                                NetworkStream strm = new NetworkStream(_clientSocket);
                                Writer.Write("32");
                                Writer.Flush();
                                formatter.Serialize(strm, users);
                                strm.Close();
                                break;
                            case PLAY_GAME:
                                SearchAnyGamersAsync();
                                break;
                            case NOTIFICATION_ABOUT_USER_CAN_ACCEPT_NEW_MESSAGE:
                                IsCanNextMessage = Reader.ReadBoolean();
                                break;
                            case PlLAYER_S_MOVE:
                                string stringPosition = Reader.ReadString();
                                int row = stringPosition[0] - '0';
                                int column = stringPosition[1] - '0';
                                _gameLogic.Move(row, column);
                                break;
                            case OUT_THE_GAME:
                                ExitFromGame();
                                break;
                        }
                    }
                    catch
                    {
                        ExitFromGame();
                        message = GetNotification("Exit");
                        _server.BroadcastMessageAboutEnteringUser(message, Id, "29");
                        _userService.UpdatePastOnlineAsync(Id, DateTime.Now);
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

        private void SetGender(string gender)
        {
            if (gender == "woman")
            {
                _human = new Woman();
            }
            else
            {
                _human = new Man();
            }
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

        private async void SearchAnyGamersAsync()
        {
            string str = await GameCenter.ConnectAsync(this);
            if (str != "")
            {
                if (str == "Have gamer")
                {
                    Writer.Write("1-11");
                    Writer.Write(_gameLogic.FirstGamer != this ? _gameLogic.FirstGamer.Name : _gameLogic.SecondGamer.Name);
                    Writer.Write(_gameLogic.FirstGamer == this ? _gameLogic.FirstGamerSymbol : _gameLogic.SecondGamerSymbol);
                    Console.WriteLine("Топчик!");
                }
                else if (str == "Have not gamer")
                {
                    Writer.Write("2-11");
                    Console.WriteLine("Никто не хочет пока играть!");
                }
                Writer.Flush();
            }
            else
            {
                Console.WriteLine("Ожидающий игрок вышел!");
            }
        }

        public void SetGameLogic(GameLogic gameLogic)
        {
            _gameLogic = gameLogic;
        }

        public void GameOpponentMove(string position)
        {
            Writer.Write("11-0");
            Writer.Write(position);
            Writer.Flush();
        }

        public void GameOver(string typeOfGameOver, string reasonGameOver)
        {
            Writer.Write(typeOfGameOver);
            Writer.Write(reasonGameOver);
            Writer.Flush();
            _gameLogic = null;
        }

        public void EarlyVictory()
        {
            Writer.Write("11-4");
            Writer.Flush();
            _gameLogic = null;
        }

        private void ExitFromGame()
        {
            if (_gameLogic == null)
            {
                GameCenter.OutTheGame(this);
            }
            else
            {
                _gameLogic.EarlyGameOver(this);
                _gameLogic = null;
            }
        }
    }
}