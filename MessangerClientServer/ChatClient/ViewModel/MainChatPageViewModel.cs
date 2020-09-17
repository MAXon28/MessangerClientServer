using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ChatClient.Interface;
using ChatClient.Logic;
using ChatClient.Logic.MessageLogic;
using ChatClient.Logic.NotificationLogic;
using ChatClient.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel
{
    class MainChatPageViewModel : ViewModelBase, IViewModel
    {
        private ServerWorker _serverWorker;
        private ChatView _chatView;
        private ChatViewModel _chatViewModel;
        private IViewModel _currentViewModel;

        public MainChatPageViewModel()
        {
        }

        public MainChatPageViewModel(string name)
        {
            NotificationTranslator.CreateNotifier();
            NotificationTranslator.GetEnteringUserNotification($"Добро пожаловать, {name}!", "Success");

            Name = name;
            _serverWorker = ServerWorker.NewInstance();

            Condition = "Visible";

            LoadEnable(true, true, false, true, true);
        }

        public string Condition { get; set; }

        public string Name { get; set; }

        public bool IsCanClickMyPage { get; set; }

        public bool IsCanClickAllUsers { get; set; }

        public bool IsCanClickChat { get; set; }

        public bool IsCanClickMiniGame { get; set; }

        public bool IsCanClickSettings { get; set; }

        public ICommand OpenMyPage
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    Name = _currentViewModel.Name;
                    _currentViewModel.Condition = "Collapsed";
                    if (_currentViewModel is ChatViewModel)
                    {
                        MessagesContainer.SaveMessages();
                    }

                    var myPageViewModel = new MyPageViewModel(Name);
                    _currentViewModel = myPageViewModel;
                    _serverWorker.RewriteDelegate(myPageViewModel);
                    await Task.Run(_serverWorker.EventOpenNewPage);

                    Content = new MyPageView();
                    Content.DataContext = myPageViewModel;

                    LoadEnable(false, true, true, true, true);
                });
            }
        }

        public ICommand OpenAllUsers
        {
            get
            {
                return new RelayCommand(async() =>
                {
                    Name = _currentViewModel.Name;
                    _currentViewModel.Condition = "Collapsed";
                    if (_currentViewModel is ChatViewModel)
                    {
                        MessagesContainer.SaveMessages();
                    }

                    var usersPageViewModel = new UsersPageViewModel(Name);
                    _currentViewModel = usersPageViewModel;
                    _serverWorker.RewriteDelegate(usersPageViewModel);
                    await Task.Run(_serverWorker.EventOpenNewPage);

                    Content = new UsersPageView();
                    Content.DataContext = usersPageViewModel;

                    await Task.Run(_serverWorker.GetAlUsers);

                    LoadEnable(true, false, true, true, true);
                });
            }
        }

        public ICommand OpenChat
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    Name = _currentViewModel.Name;
                    _currentViewModel.Condition = "Collapsed";

                    _chatViewModel.Condition = "Visible";
                    _chatViewModel.Name = Name;
                    _currentViewModel = _chatViewModel;
                    _serverWorker.RewriteDelegate(_chatViewModel);
                    await Task.Run(_serverWorker.EventOpenNewPage);

                    Content = _chatView;
                    Content.DataContext = _chatViewModel;
                    _chatViewModel.UsuallyLoad();

                    LoadEnable(true, true, false, true, true);
                });
            }
        }

        public FrameworkElement Content { get; set; }

        public void Notification(BinaryReader binaryReader)
        {
            string code = binaryReader.ReadString();
            //_notifier.ShowInformation(binaryReader.ReadString());
        }

        public async void StartLoad()
        {
            _chatView = new ChatView();
            _chatViewModel = new ChatViewModel(_chatView, Name);
            _currentViewModel = _chatViewModel;
            await Task.Run(() => _chatViewModel.StartLoad());
            _serverWorker.RewriteDelegate(_chatViewModel);
            Content = _chatView;
            Content.DataContext = _chatViewModel;

            _chatView.ScrollStart();
        }

        private void LoadEnable(bool isCanClickMyPage, bool isCanClickAllUsers, bool isCanClickChat, bool isCanClickMiniGame, bool isCanClickSettings)
        {
            IsCanClickMyPage = isCanClickMyPage;
            IsCanClickAllUsers = isCanClickAllUsers;
            IsCanClickChat = isCanClickChat;
            IsCanClickMiniGame = isCanClickMiniGame;
            IsCanClickSettings = isCanClickSettings;
        }
    }
}