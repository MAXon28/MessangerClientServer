using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ChatClient.Interface;
using ChatClient.Logic;
using ChatClient.Logic.MessageLogic;
using ChatClient.Logic.NotificationLogic;
using ChatClient.View;
using ChatClient.View.Dialog;
using ChatClient.View.Game;
using ChatClient.ViewModel.Dialog;
using ChatClient.ViewModel.Game;
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

        public MainChatPageViewModel() { }

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
                    if (_currentViewModel is ChatViewModel)
                    {
                        MessagesContainer.SaveMessages();
                    }
                    else if (_currentViewModel is GameMainPageViewModel)
                    {
                        var gameViewModel = (GameMainPageViewModel) _currentViewModel;
                        if (gameViewModel.ViewModel != null && gameViewModel.ViewModel.Condition != "Collapsed" && !gameViewModel.ViewModel.IsVisibleCancel)
                        {
                            if (gameViewModel.ViewModel.IsVisibleGame)
                            {
                                var gameDialogViewModel = new GameDialogViewModel();
                                var gameDialogView = new GameDialogView(gameDialogViewModel);
                                gameDialogView.ShowDialog();
                                if (gameDialogViewModel.UserResponse != "Да")
                                {
                                    return;
                                }
                                if (gameViewModel.ViewModel.TypeOfGame == "With user")
                                {
                                    await Task.Run(_serverWorker.OutTheGame);
                                }
                                else
                                {
                                    await Task.Run(() => _serverWorker.GameOverThisComputer(1102));
                                }
                            }
                            else if (gameViewModel.ViewModel.IsVisibleSpinner)
                            {
                                await Task.Run(_serverWorker.OutTheGame);
                            }
                        }
                    }

                    _currentViewModel.Condition = "Collapsed";

                    var myPageViewModel = new MyPageViewModel(Name);
                    _currentViewModel = myPageViewModel;
                    _serverWorker.RewriteDelegate(myPageViewModel);
                    await Task.Run(() => _serverWorker.EventOpenNewPage());

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
                    if (_currentViewModel is ChatViewModel)
                    {
                        MessagesContainer.SaveMessages();
                    }
                    else if (_currentViewModel is GameMainPageViewModel)
                    {
                        var gameViewModel = (GameMainPageViewModel)_currentViewModel;
                        if (gameViewModel.ViewModel != null && gameViewModel.ViewModel.Condition != "Collapsed" && !gameViewModel.ViewModel.IsVisibleCancel)
                        {
                            if (gameViewModel.ViewModel.IsVisibleGame)
                            {
                                var gameDialogViewModel = new GameDialogViewModel();
                                var gameDialogView = new GameDialogView(gameDialogViewModel);
                                gameDialogView.ShowDialog();
                                if (gameDialogViewModel.UserResponse != "Да")
                                {
                                    return;
                                }
                                if (gameViewModel.ViewModel.TypeOfGame == "With user")
                                {
                                    await Task.Run(_serverWorker.OutTheGame);
                                }
                                else
                                {
                                    await Task.Run(() => _serverWorker.GameOverThisComputer(1102));
                                }
                            }
                            else if (gameViewModel.ViewModel.IsVisibleSpinner)
                            {
                                await Task.Run(_serverWorker.OutTheGame);
                            }
                        }
                    }

                    _currentViewModel.Condition = "Collapsed";

                    var usersPageViewModel = new UsersPageViewModel(Name);
                    _currentViewModel = usersPageViewModel;
                    _serverWorker.RewriteDelegate(usersPageViewModel);
                    await Task.Run(() => _serverWorker.EventOpenNewPage());

                    Content = new UsersPageView();
                    Content.DataContext = usersPageViewModel;

                    await Task.Run(_serverWorker.GetAllUsers);

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
                    if (_currentViewModel is GameMainPageViewModel)
                    {
                        var gameViewModel = (GameMainPageViewModel)_currentViewModel;
                        if (gameViewModel.ViewModel != null && gameViewModel.ViewModel.Condition != "Collapsed" && !gameViewModel.ViewModel.IsVisibleCancel)
                        {
                            if (gameViewModel.ViewModel.IsVisibleGame)
                            {
                                var gameDialogViewModel = new GameDialogViewModel();
                                var gameDialogView = new GameDialogView(gameDialogViewModel);
                                gameDialogView.ShowDialog();
                                if (gameDialogViewModel.UserResponse != "Да")
                                {
                                    return;
                                }
                                if (gameViewModel.ViewModel.TypeOfGame == "With user")
                                {
                                    await Task.Run(_serverWorker.OutTheGame);
                                }
                                else
                                {
                                    await Task.Run(() => _serverWorker.GameOverThisComputer(1102));
                                }
                            }
                            else if (gameViewModel.ViewModel.IsVisibleSpinner)
                            {
                                await Task.Run(_serverWorker.OutTheGame);
                            }
                        }
                    }

                    _currentViewModel.Condition = "Collapsed";

                    _chatViewModel.Condition = "Visible";
                    _chatViewModel.Name = Name;
                    _currentViewModel = _chatViewModel;
                    _serverWorker.RewriteDelegate(_chatViewModel);
                    await Task.Run(() => _serverWorker.EventOpenNewPage("Chat page"));

                    Content = _chatView;
                    Content.DataContext = _chatViewModel;
                    _chatViewModel.UsuallyLoadAsync();

                    LoadEnable(true, true, false, true, true);
                });
            }
        }

        public ICommand OpenMiniGame
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

                    var gameMainPageViewModel = new GameMainPageViewModel(Name);
                    _currentViewModel = gameMainPageViewModel;
                    _serverWorker.RewriteDelegate(gameMainPageViewModel);
                    await Task.Run(() => _serverWorker.EventOpenNewPage());

                    Content = new GameMainPageView();
                    Content.DataContext = gameMainPageViewModel;

                    LoadEnable(true, true, true, false, true);
                });
            }
        }

        public ICommand OpenSettings
        {
            get
            {
                return new RelayCommand( async () =>
                {
                    Name = _currentViewModel.Name;
                    if (_currentViewModel is ChatViewModel)
                    {
                        MessagesContainer.SaveMessages();
                    }
                    else if (_currentViewModel is GameMainPageViewModel)
                    {
                        var gameViewModel = (GameMainPageViewModel)_currentViewModel;
                        if (gameViewModel.ViewModel != null && gameViewModel.ViewModel.Condition != "Collapsed" && !gameViewModel.ViewModel.IsVisibleCancel)
                        {
                            if (gameViewModel.ViewModel.IsVisibleGame)
                            {
                                var gameDialogViewModel = new GameDialogViewModel();
                                var gameDialogView = new GameDialogView(gameDialogViewModel);
                                gameDialogView.ShowDialog();
                                if (gameDialogViewModel.UserResponse != "Да")
                                {
                                    return;
                                }
                                if (gameViewModel.ViewModel.TypeOfGame == "With user")
                                {
                                    await Task.Run(_serverWorker.OutTheGame);
                                }
                                else
                                {
                                    await Task.Run(() => _serverWorker.GameOverThisComputer(1102));
                                }
                            }
                            else if (gameViewModel.ViewModel.IsVisibleSpinner)
                            {
                                await Task.Run(_serverWorker.OutTheGame);
                            }
                        }
                    }

                    _currentViewModel.Condition = "Collapsed";

                    var settingsPageViewModel = new SettingsPageViewModel(Name);
                    _currentViewModel = settingsPageViewModel;
                    _serverWorker.RewriteDelegate(settingsPageViewModel);
                    await Task.Run(() => _serverWorker.EventOpenNewPage());

                    Content = new SettingsPageView();
                    Content.DataContext = settingsPageViewModel;

                    LoadEnable(true, true, true, true, false);
                });
            }
        }

        public ICommand Exit
        {
            get
            {
                return new RelayCommand( async () =>
                {
                    if (_currentViewModel is ChatViewModel)
                    {
                        MessagesContainer.SaveMessages();
                    }
                    else if (_currentViewModel is GameMainPageViewModel)
                    {
                        var gameViewModel = (GameMainPageViewModel)_currentViewModel;
                        if (gameViewModel.ViewModel != null && gameViewModel.ViewModel.Condition != "Collapsed" && !gameViewModel.ViewModel.IsVisibleCancel)
                        {
                            if (gameViewModel.ViewModel.IsVisibleGame)
                            {
                                var gameDialogViewModel = new GameDialogViewModel();
                                var gameDialogView = new GameDialogView(gameDialogViewModel);
                                gameDialogView.ShowDialog();
                                if (gameDialogViewModel.UserResponse != "Да")
                                {
                                    return;
                                }

                                if (gameViewModel.ViewModel.TypeOfGame == "With user")
                                {
                                    await Task.Run(_serverWorker.OutTheGame);
                                }
                                else
                                {
                                    await Task.Run(() => _serverWorker.GameOverThisComputer(1102));
                                }
                            }
                            else if (gameViewModel.ViewModel.IsVisibleSpinner)
                            {
                                await Task.Run(_serverWorker.OutTheGame);
                            }
                        }
                    }
                    _serverWorker.Exit();
                    _serverWorker.Close();
                    ServerWorker.SetNull();
                    Condition = "Collapsed";
                });
            }
        }

        public FrameworkElement Content { get; set; }

        public void Notification(BinaryReader binaryReader)
        {
            //ignore
        }

        public async void StartLoad(bool isHavePastMessage)
        {
            _chatView = new ChatView();
            _chatViewModel = new ChatViewModel(_chatView, Name);
            _currentViewModel = _chatViewModel;
            await Task.Run(() => _chatViewModel.StartLoad(isHavePastMessage));
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