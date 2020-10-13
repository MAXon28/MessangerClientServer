using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
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
        private GameService _gameService;
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
        private const int GET_GAME_RATING = 13;
        private const int UPDATE_SETTINGS = 14;
        private const int PlLAYER_S_MOVE = 110;
        private const int OUT_THE_GAME = 111;
        private const int WIN_IN_GAME_WITH_COMPUTER = 1101;
        private const int LOSE_IN_GAME_WITH_COMPUTER = 1102;
        private const int DRAW_IN_GAME_WITH_COMPUTER = 1103;

        public Client(Socket clientSocket, ServerObject server, UserService userService, Guid id, string gender, string name)
        {
            _clientSocket = clientSocket;
            _server = server;
            _userService = userService;
            _chatService = new ChatService(userService.GetUnitOfWork());
            _gameService = new GameService(_chatService.GetUnitOfWork());
            _settingsService = new SettingsService(_gameService.GetUnitOfWork());
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

        public int PastIdMessageWhichCanSee { get; set; }

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
                                RewriteUserName();
                                break;
                            case REWRITE_USER_GENDER:
                                RewriteUserGender();
                                break;
                            case REWRITE_USER_LOGIN:
                                RewriteUserLogin();
                                break;
                            case REWRITE_USER_PASSWORD:
                                RewriteUserPasswordAsync();
                                break;
                            case EXIT:
                                Exit();
                                break;
                            case NOTIFICATION_ABOUT_GO_TO_NEW_PAGE:
                                NotificationAboutGoToNewPage();
                                break;
                            case LOAD_USERS:
                                LoadUsers();
                                break;
                            case PLAY_GAME:
                                SearchAnyGamersAsync();
                                break;
                            case NOTIFICATION_ABOUT_USER_CAN_ACCEPT_NEW_MESSAGE:
                                NotificationAboutUserCanAcceptNewMessage();
                                break;
                            case GET_GAME_RATING:
                                GetGameRatingAsync();
                                break;
                            case UPDATE_SETTINGS:
                                UpdateSettings();
                                break;
                            case PlLAYER_S_MOVE:
                                PlayerSMove();
                                break;
                            case OUT_THE_GAME:
                                ExitFromGame();
                                break;
                            case WIN_IN_GAME_WITH_COMPUTER:
                                WinInGameWithComputer();
                                break;
                            case LOSE_IN_GAME_WITH_COMPUTER:
                                LoseInGameWithComputer();
                                break;
                            case DRAW_IN_GAME_WITH_COMPUTER:
                                DrawInGameWithComputer();
                                break;
                        }
                    }
                    catch
                    {
                        ExitFromGame();
                        message = GetNotification("Exit");
                        _server.BroadcastMessageAboutEnteringUser(message, Id, "29");
                        _userService.UpdatePastOnlineAndSeeMessageIdAsync(Id, DateTime.Now, PastIdMessageWhichCanSee);
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

        public void Close()
        {
            Writer?.Close();
            Reader?.Close();
            _clientSocket?.Close();
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
            PastMessage = _chatService.AddMessage(Id, timeSendMessage, message, Name);
            PastIdMessageWhichCanSee = PastMessage.Id;
            _server.BroadcastMessageFromUser("30", Name, message, timeSendMessage.ToString(), PastMessage.Id, Id);
        }

        private void GetMessages()
        {
            IFormatter formatter = new BinaryFormatter();
            NetworkStream strm = new NetworkStream(_clientSocket);
            List<ChatMessageDTO> list = new List<ChatMessageDTO>();
            string typeRequest = Reader.ReadString();
            if (typeRequest == "New entering in chat")
            {
                list = _chatService.GetCurrentReadMessages(PastIdMessageWhichCanSee);
                int count = list.Count;
                (List<ChatMessageDTO>, int) data = _chatService.GetNewMessagesAndAllCountNotReadMessages(list);
                list.AddRange(data.Item1);
                PastMessage = data.Item1.Count - 1 >= 0 ? data.Item1[data.Item1.Count - 1] : list[list.Count - 1];
                PastIdMessageWhichCanSee = PastMessage.Id;
                Writer.Write(data.Item2);
                Writer.Write(count);
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
                PastIdMessageWhichCanSee = PastMessage.Id;
            }
            else if(typeRequest == "New messages")
            {
                if (PastMessage?.Id == Reader.ReadInt32())
                {
                    (List<ChatMessageDTO>, int) data = _chatService.GetNewMessagesAndAllCountNotReadMessages(PastMessage);
                    list = data.Item1;
                    PastMessage = list.Count > 0 ? list[list.Count - 1] : PastMessage;
                    PastIdMessageWhichCanSee = PastMessage.Id;
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
                PastIdMessageWhichCanSee = PastMessage.Id;
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

        private void RewriteUserName()
        {
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
            Writer.Flush();
        }

        private void RewriteUserGender()
        {
            string newGender = Reader.ReadString();
            SetGender(newGender);
            _userService.UpdateUserAsync(2, Id, newGender);
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

        private void RewriteUserLogin()
        {
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
            Writer.Flush();
        }

        private async void RewriteUserPasswordAsync()
        {
            await Task.Run(() => _userService.UpdateUserAsync(4, Id, Reader.ReadString()));
        }

        private void Exit()
        {
            _server.RemoveConnection(Id);
            Close();
        }

        private void NotificationAboutGoToNewPage()
        {
            UserActivePage = Reader.ReadString();
            Writer.Write("");
            Writer.Flush();
        }

        private void LoadUsers()
        {
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

        private void PlayerSMove()
        {
            int row = Reader.ReadInt32();
            int column = Reader.ReadInt32();
            _gameLogic.Move(row, column);
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
            int intTypeOfGameOver = GetIntTypeOfGameOver(typeOfGameOver);
            _gameService.UpdateStatistic(Id, 1, intTypeOfGameOver);
        }

        public void EarlyVictory()
        {
            Writer.Write("11-4");
            Writer.Flush();
            _gameLogic = null;
            _gameService.UpdateStatistic(Id, 1, 1);
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
                _gameService.UpdateStatistic(Id, 1, 2);
            }
        }

        private int GetIntTypeOfGameOver(string stringTypeOfGameOver) =>
            stringTypeOfGameOver switch
            {
                "11-1" => 1,
                "11-2" => 2,
                "11-3" => 3,
                _ => -1
            };

        private void NotificationAboutUserCanAcceptNewMessage()
        {
            IsCanNextMessage = Reader.ReadBoolean();
        }

        private async void GetGameRatingAsync()
        {
            (List<RatingDTO>, List<RatingDTO>, List<RatingDTO>) ratings = await _gameService.GetGameRatingsAsync();
            IFormatter formatter = new BinaryFormatter();
            NetworkStream strm = new NetworkStream(_clientSocket);
            Writer.Write("13-1");
            Writer.Flush();
            formatter.Serialize(strm, ratings.Item1);
            formatter.Serialize(strm, ratings.Item2);
            formatter.Serialize(strm, ratings.Item3);
            strm.Close();
        }

        private void UpdateSettings()
        {
            string data = Reader.ReadString();
            if (int.TryParse(data, out var typeOfSound))
            {
                _settingsService.UpdateData(Id, typeOfSound);
            }
            else
            {
                _settingsService.UpdateData(Id, data);
            }
        }

        private void WinInGameWithComputer()
        {
            _gameService.UpdateStatistic(Id, 2, 1);
        }

        private void LoseInGameWithComputer()
        {
            _gameService.UpdateStatistic(Id, 2, 2);
        }

        private void DrawInGameWithComputer()
        {
            _gameService.UpdateStatistic(Id, 2, 3);
        }
    }
}