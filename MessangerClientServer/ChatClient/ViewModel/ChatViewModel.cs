using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AutoMapper;
using ChatClient.Interface;
using ChatClient.Logic;
using ChatClient.Logic.MessageLogic;
using ChatClient.Logic.NotificationLogic;
using ChatClient.Logic.UserLogic;
using ChatClient.Model;
using ChatClient.View;
using ChatLibrary;
using ChatLibrary.DataTransferObject;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel
{
    /// <summary>
    /// Отвечает за тип загрузки сообщения: 1 - загрузка прошлых сообщений, 2 - загрузка новых сообщений
    /// </summary>
    enum TypeOfLoadMessage
    {
        PastMessages = 1,
        NewMessages = 2
    }

    class ChatViewModel : ViewModelBase, IViewModel
    {
        private ChatView _chatView;
        private ServerWorker _serverWorker;
        private readonly Cache _cache;
        private int _topID;

        /// <summary>
        /// Источник, откуда загружаются актуальные сообщения: 0 - не нужно загружать, 1 - загрузка из сервера, 2 - загрузка из кэша
        /// </summary>
        private int _typeOfSource;

        /// <summary>
        /// Написал ли данные пользователь новое сообщение (используется как флаг)
        /// </summary>
        private bool _haveNewMessage;

        /// <summary>
        /// Идёт ли прогрузка вниз (используется как флаг)
        /// </summary>
        private bool _isGoToBottom;

        /// <summary>
        /// Количество нажайти на кнопку "Вниз" подряд
        /// </summary>
        private int _countClickButtonToBottom;

        public ChatViewModel() { }

        public ChatViewModel(ChatView chatView, string name)
        {

            _chatView = chatView;
            _serverWorker = ServerWorker.NewInstance();
            _cache = new Cache(name);
            Name = name;

            Condition = "Visible";
            Index = 0;
            IsFocus = true;
            IsVisibleNotification = false;
            IsVisibleButtonToBottom = false;
            CountNewMessages = 0;

            _haveNewMessage = false;
            _isGoToBottom = false;
            _countClickButtonToBottom = 0;
        }

        public void StartLoad(bool isHavePastMessage)
        {
            Messages = new ObservableCollection<Message>();
            MessagesContainer.OpenContainer(Name);
            List<ChatMessageDTO> chatMessagesServer = new List<ChatMessageDTO>();
            if (isHavePastMessage)
            {
                (List<ChatMessageDTO>, int, int) data = _serverWorker.GetMessagesStartWithPastMessage();
                chatMessagesServer = data.Item1;
                _topID = chatMessagesServer[0].Id;
                for (int i = 0; i < data.Item3; i++)
                {
                    Messages.Add(new Message
                    {
                        Id = chatMessagesServer[i].Id,
                        SenderName = chatMessagesServer[i].SenderName,
                        DateSend = chatMessagesServer[i].DateSend,
                        SendMessage = chatMessagesServer[i].SendMessage,
                        IsItMe = chatMessagesServer[i].SenderName == Name
                    });
                    AddNewMessageInContainerAndUpdateDateSend();
                }
                if (chatMessagesServer.Count > data.Item3)
                {
                    _typeOfSource = 1;
                    MessagesLoadAsync(chatMessagesServer.Skip(data.Item3).ToList(), data.Item2, data.Item3 - 1);
                }
                else
                {
                    _typeOfSource = 0;
                }
            }
            else
            {
                chatMessagesServer = _serverWorker.GetMessagesStart();

                _topID = chatMessagesServer.Count > 0 ? chatMessagesServer[0].Id : -1;
                _typeOfSource = 0;

                foreach (var message in chatMessagesServer)
                {
                    Messages.Add(new Message
                    {
                        Id = message.Id,
                        SenderName = message.SenderName,
                        DateSend = message.DateSend,
                        SendMessage = message.SendMessage,
                        IsItMe = message.SenderName == Name
                    });
                    AddNewMessageInContainerAndUpdateDateSend();
                }
            }
        }

        public async void UsuallyLoadAsync()
        {
            Messages = new ObservableCollection<Message>();
            List<Message> chatMessagesClient = MessagesContainer.GetMessages();
            foreach (var message in chatMessagesClient)
            {
                if (message.IsItMe && message.SenderName != Name)
                {
                    message.SenderName = Name;
                }
                message.DateSend = Day.GetParsedDate(message.DateSend);
                Messages.Add(message);
            }

            if (Messages.Count > 0)
            {
                await Task.Run(() => _serverWorker.GetMessages(Messages[Messages.Count - 1].Id, "New messages"));
            }
        }

        public ObservableCollection<Message> Messages { get; set; }

        public string Condition { get; set; }

        public int Index { get; set; }

        public string Message { get; set; }

        public bool IsFocus { get; set; }

        public bool IsVisibleNotification { get; set; } 

        public bool IsVisibleButtonToBottom { get; set; }

        public int CountNewMessages { get; set; }

        public double Opacity { get; set; }

        public string Name { get; set; }

        public ICommand Send
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (!string.IsNullOrEmpty(Message))
                    {
                        if (_typeOfSource == 1)
                        {
                            Messages.Clear();
                            if (CountNewMessages < 100 && _cache.IndexLevel > 0)
                            {
                                _cache.DeserializeMessages(0);
                            }
                            CountNewMessages = 0;
                            _haveNewMessage = true;
                            await Task.Run(_serverWorker.GetMessagesActual);
                        }
                        else if (_typeOfSource == 2)
                        {
                            Messages = new ObservableCollection<Message>(_cache.DeserializeMessages(0));
                        }
                        _typeOfSource = 0;
                        if (!_haveNewMessage)
                        {
                            Messages.Add(new Message()
                            {
                                Id = Messages.Count > 0 ? Messages[Messages.Count - 1].Id + 1 : 1,
                                SenderName = Name,
                                DateSend = DateTime.Now.ToString(),
                                SendMessage = Message,
                                IsItMe = true
                            });
                            await Task.Run(() => _serverWorker.SendMessage(Message));
                            AddNewMessageInContainerAndUpdateDateSend();
                            _chatView.Scroll();
                            _topID = Messages[0].Id;
                            Message = "";
                        }
                        _countClickButtonToBottom = 0;
                        IsVisibleButtonToBottom = false;
                        await Task.Run(() => _serverWorker.GetUpdate(!IsVisibleButtonToBottom));
                    }
                });
            }
        }

        public async void UpdateMessagesAsync()
        {
            IsFocus = false;
            if (!_haveNewMessage && !_isGoToBottom)
            {
                await Task.Run(() => _serverWorker.GetMessages(_topID, "Have ID"));
            }
        }

        public ICommand CopyText
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Clipboard.SetText(Messages[Index].SendMessage);
                    ShowNotificationAsync();
                });
            }
        }

        /// <summary>
        /// Срабатывает при нажатии на кнопку "Вниз"
        /// </summary>
        public ICommand GoToBottom
        {
            get
            {
                return new RelayCommand( async () =>
                {
                    if (_typeOfSource == 2 && CountNewMessages == 0) // Если новых сообщений нет, но есть закэшированные данные
                    {
                        Messages = new ObservableCollection<Message>(_cache.DeserializeMessages(0));
                        _chatView.Scroll();
                    }
                    if (CountNewMessages > 0 && _countClickButtonToBottom == 1) // Если есть новые сообщения и кнопка нажимается второй раз подряд
                    {
                        _isGoToBottom = true;
                        Messages.Clear();
                        if (CountNewMessages < 200 && _cache.IndexLevel > 0)
                        {
                            Messages = new ObservableCollection<Message>(_cache.DeserializeMessages(0));
                            await Task.Run(() => _serverWorker.GetMessages(Messages[Messages.Count - 1].Id, "New messages"));
                        }
                        else
                        {
                            await Task.Run(_serverWorker.GetMessagesActual);
                        }
                        _countClickButtonToBottom = 0;
                    }
                    else if (CountNewMessages > 0 && _countClickButtonToBottom == 0)
                    {
                        if (CountNewMessages < 200)
                        {
                            if (_cache.IndexLevel > 0)
                            {
                                Messages.Clear();
                                Messages = new ObservableCollection<Message>(_cache.DeserializeMessages(0));
                            }
                            _chatView.Scroll(Messages.Count - 5);
                            _countClickButtonToBottom++;
                            return;
                        }
                        _serverWorker.GetMessages(Messages[Messages.Count - 1].Id, "New messages");
                        _countClickButtonToBottom++;
                        _isGoToBottom = true;
                        return;
                    }
                    else // Иначе просто прокручивается до самого нового сообщения
                    {
                        _chatView.Scroll();
                    }
                    _typeOfSource = 0;
                    _countClickButtonToBottom = 0;
                    CountNewMessages = 0;
                    IsVisibleButtonToBottom = false;
                    await Task.Run(() => _serverWorker.GetUpdate(!IsVisibleButtonToBottom));
                });
            }
        }

        /// <summary>
        /// Отображает или убирает кнопку "Вниз"
        /// </summary>
        /// <param name="isVisible"></param>
        public async void ShowButtonToBottomAsync(bool isVisible)
        {
            IsFocus = false;
            if (_cache.IndexLevel == 0 && CountNewMessages == 0 && !_haveNewMessage)
            {
                IsVisibleButtonToBottom = isVisible;
                await Task.Run(() => _serverWorker.GetUpdate(!IsVisibleButtonToBottom));
                if (IsVisibleButtonToBottom)
                {
                    GetFlickerAsync();
                }
                else
                {
                    CountNewMessages = 0;
                }
                _countClickButtonToBottom = 0;
                IsFocus = true;
            }
            else if (_cache.IndexLevel > 0 && _isGoToBottom == false)
            {
                _isGoToBottom = true;
                Application.Current.Dispatcher.Invoke(LoadFromCache, DispatcherPriority.Background);
                _countClickButtonToBottom = 0;
                IsFocus = true;
            }
            else if (_cache.IndexLevel > 0)
            {
                _isGoToBottom = false;
                IsFocus = true;
            }
            else if (CountNewMessages > 0 && _isGoToBottom == false)
            {
                if (CountNewMessages > 100)
                {
                    await Task.Run(() => _serverWorker.GetMessages(Messages[Messages.Count - 1].Id, "New messages"));
                }
                else
                {
                    CountNewMessages = 0;
                    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Message, ChatMessageDTO>()).CreateMapper();
                    await Task.Run(() => _serverWorker.GetMessages(mapper.Map<Message, ChatMessageDTO>(MessagesContainer.GetMessage())));
                }
                _countClickButtonToBottom = 0;
            }
            else
            {
                _isGoToBottom = false;
            }
        }

        private void LoadFromCache()
        {
            List<Message> list = _cache.DeserializeMessages(1);
            if (_cache.IndexLevel == 0)
            {
                _typeOfSource = 0;
            }
            foreach (var message in list)
            {
                Messages.Add(message);
            }
            DeletePastMessages();
            _chatView.Scroll(Messages.Count - list.Count);
            _topID = Messages[0].Id;
        }

        public void Notification(BinaryReader binaryReader)
        {
            string code = binaryReader.ReadString();
            switch (code)
            {
                case "29":
                    NotificationTranslator.GetEnteringUserNotification(binaryReader.ReadString(), "Information");
                    break;
                case "30":
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NewMessage(binaryReader);
                    }, DispatcherPriority.Background);
                    break;
                case "31":
                    Application.Current.Dispatcher.Invoke(PastMessages, DispatcherPriority.Background);
                    break;
                case "40":
                    NotificationTranslator.RewriteDataNotification("Имя пользователя изменено!", "Success");
                    Name = binaryReader.ReadString();
                    break;
                case "41":
                    NotificationTranslator.RewriteDataNotification("Имя пользователя не изменено! Пользователь с таким именем уже зарегистрирован!", "Error");
                    break;
                case "42":
                    NotificationTranslator.RewriteDataNotification("Логин пользователя изменён!", "Success");
                    UserContainer.Login = binaryReader.ReadString();
                    break;
                case "43":
                    NotificationTranslator.RewriteDataNotification("Логин пользователя не изменён! Пользователь с таким логином уже зарегистрирован!", "Error");
                    break;
                case "67":
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        NewMessages(binaryReader);
                    }, DispatcherPriority.Background);
                    break;
            }
        }

        private async void MessagesLoadAsync(List<ChatMessageDTO> chatMessagesServer, int count, int index)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var message in chatMessagesServer)
                    {
                        Messages.Add(new Message
                        {
                            Id = message.Id,
                            SenderName = message.SenderName,
                            DateSend = message.DateSend,
                            SendMessage = message.SendMessage,
                            IsItMe = message.SenderName == Name
                        });
                        AddNewMessageInContainerAndUpdateDateSend();
                    }
                    IsVisibleButtonToBottom = true;
                    GetFlickerAsync();
                    _serverWorker.GetUpdate(!IsVisibleButtonToBottom);
                    CountNewMessages = count;
                    _chatView.Scroll(index);

                }, DispatcherPriority.Background);
            });
        }

        /// <summary>
        /// Отображает уведомление о скопированном тексте
        /// </summary>
        private async void ShowNotificationAsync()
        {
            IsVisibleNotification = true;
            await Task.Delay(1928);
            IsVisibleNotification = false;
        }

        /// <summary>
        /// Постепенно делает прозрачным кнопку "Вниз"
        /// </summary>
        private async void GetFlickerAsync()
        {
            Opacity = 1;
            for (int i = 0; i < 9; i++)
            {
                await Task.Delay(128);
                Opacity = i % 2 == 0 ? Opacity - 0.05 : Opacity - 0.02;
            }
        }

        /// <summary>
        /// Получение нового сообщения со стороны сервера
        /// </summary>
        /// <param name="binaryReader"></param>
        private void NewMessage(BinaryReader binaryReader)
        {
            string stringAnswer = binaryReader.ReadString();
            if (stringAnswer != "+1")
            {
                Messages.Add(new Message()
                {
                    Id = Messages.Count > 0 ? Messages[Messages.Count - 1].Id + 1 : 1,
                    SenderName = stringAnswer,
                    SendMessage = binaryReader.ReadString(),
                    DateSend = binaryReader.ReadString(),
                    IsItMe = false
                });
                AddNewMessageInContainerAndUpdateDateSend();
                _chatView.Scroll();
            }
            else
            {
                CountNewMessages++;
                _typeOfSource = 1;
            }
        }

        /// <summary>
        /// Подгрузка прошлых сообщений
        /// </summary>
        private void PastMessages()
        {
            if (Messages.Count >= 400)
            {
                _typeOfSource = 2;
                _cache.SerializeMessages(Messages.Skip(Messages.Count - 200).ToList());
                int count = 0;
                while (count < 200)
                {
                    Messages.RemoveAt(Messages.Count - 1);
                    count++;
                }
            }

            AddMessages(TypeOfLoadMessage.PastMessages);
            _topID = Messages[0].Id;

            IsFocus = true;
        }

        /// <summary>
        /// Подгрузка новых сообщений
        /// </summary>
        /// <param name="binaryReader"> Данные, полученные из сервера </param>
        private void NewMessages(BinaryReader binaryReader)
        {
            CountNewMessages = binaryReader.ReadInt32();
            _typeOfSource = CountNewMessages == 0 ? 0 : 1;
            DeletePastMessages();
            AddMessages(TypeOfLoadMessage.NewMessages);
            if (_isGoToBottom && CountNewMessages == 0)
            {
                _chatView.Scroll();
                _isGoToBottom = false;
            }

            if (_haveNewMessage)
            {
                Messages.Add(new Message()
                {
                    Id = Messages.Count > 0 ? Messages[Messages.Count - 1].Id + 1 : 1,
                    SenderName = Name,
                    DateSend = DateTime.Now.ToString(),
                    SendMessage = Message,
                    IsItMe = true
                });
                _serverWorker.SendMessage(Message);
                AddNewMessageInContainerAndUpdateDateSend();
                Message = "";
                _chatView.Scroll();
                _haveNewMessage = false;
            }
            _topID = Messages[0].Id;

            IsFocus = true;
        }

        /// <summary>
        /// Добавляет новые сообщения, которые пришли из сервера
        /// </summary>
        /// <param name="typeOfLoadMessage"> Тип загрузки сообщений </param>
        private void AddMessages(TypeOfLoadMessage typeOfLoadMessage)
        {
            IFormatter formatter = new BinaryFormatter();
            var list = (List<ChatMessageDTO>)formatter.Deserialize(_serverWorker.NetworkStream);
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    //if (Messages.IndexOf(list[i]) == -1)
                    Messages.Add(new Message()
                    {
                        Id = list[i].Id,
                        SenderName = list[i].SenderName,
                        DateSend = list[i].DateSend,
                        SendMessage = list[i].SendMessage,
                        IsItMe = list[i].SenderName == Name
                    });
                    if (typeOfLoadMessage == TypeOfLoadMessage.PastMessages)
                    {
                        Messages[Messages.Count - 1].DateSend = Day.GetParsedDate(Messages[Messages.Count - 1].DateSend);
                    }
                    else
                    {
                        AddNewMessageInContainerAndUpdateDateSend();
                    }
                }

                Messages = new ObservableCollection<Message>(from data in Messages
                    orderby data.Id
                    select data);

                if (typeOfLoadMessage == TypeOfLoadMessage.PastMessages)
                {
                    _chatView.Scroll(list.Count);
                }
                else
                {
                    _chatView.Scroll(Messages.Count - list.Count);
                }
            }
        }

        /// <summary>
        /// Загружает сообщение в контейнер и задаёт понятную для пользователя дату
        /// </summary>
        private void AddNewMessageInContainerAndUpdateDateSend()
        {
            MessagesContainer.AddMessage(Messages[Messages.Count - 1].Id,
                Messages[Messages.Count - 1].SenderName,
                Messages[Messages.Count - 1].DateSend,
                Messages[Messages.Count - 1].SendMessage,
                Messages[Messages.Count - 1].IsItMe);
            Messages[Messages.Count - 1].DateSend = Day.GetParsedDate(Messages[Messages.Count - 1].DateSend);
        }

        private void DeletePastMessages()
        {
            if (Messages.Count >= 400)
            {
                int count = 0;
                while (count < 200)
                {
                    Messages.RemoveAt(0);
                    count++;
                }
            }
        }
    }
}