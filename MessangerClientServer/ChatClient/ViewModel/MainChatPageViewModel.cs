using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ChatClient.Interface;
using ChatClient.Logic;
using ChatClient.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace ChatClient.ViewModel
{
    class MainChatPageViewModel : ViewModelBase, IViewModel
    {
        private Notifier _notifier;
        private ServerWorker _serverWorker;
        private ChatView _chatView;
        private ChatViewModel _chatViewModel;
        private MyPageViewModel _myPageViewModel;
        private string _name;

        public MainChatPageViewModel() { }

        public MainChatPageViewModel(string name)
        {
            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomLeft,
                    offsetX: 3,
                    offsetY: 167);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(3));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
            _notifier.ShowSuccess($"Добро пожаловать, {name}!");

            _name = name;
            _serverWorker = ServerWorker.NewInstance();
        }

        public ICommand OpenMyPage
        {
            get
            {
                return new RelayCommand(async() =>
                {
                    _name = _chatViewModel.Name;
                    _chatViewModel.Condition = "Collapsed";
                    MessagesContainer.SaveMessages();
                    _myPageViewModel = new MyPageViewModel(_notifier, _name);
                    _serverWorker.RewriteDelegate(_myPageViewModel);
                    await Task.Run(_serverWorker.EventOpenNewPage);
                    Content = new MyPageView();
                    Content.DataContext = _myPageViewModel;
                });
            }
        }

        public ICommand OpenChat
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    _name = _myPageViewModel.Name;
                    _myPageViewModel.Condition = "Collapsed";
                    _chatViewModel.Condition = "Visible";
                    _chatViewModel.Name = _name;
                    _serverWorker.RewriteDelegate(_chatViewModel);
                    await Task.Run(_serverWorker.EventOpenNewPage);
                    Content = _chatView;
                    Content.DataContext = _chatViewModel;
                    _chatViewModel.UsuallyLoad();
                });
            }
        }

        public FrameworkElement Content { get; set; }

        public void Notification(BinaryReader binaryReader)
        {
            string code = binaryReader.ReadString();
            _notifier.ShowInformation(binaryReader.ReadString());
        }

        public async void StartLoad()
        {
            _chatView = new ChatView();
            _chatViewModel = new ChatViewModel(_chatView, _notifier, _name);
            await Task.Run(() => _chatViewModel.StartLoad());
            _serverWorker.RewriteDelegate(_chatViewModel);
            Content = _chatView;
            Content.DataContext = _chatViewModel;

            _chatView.ScrollStart();
        }
    }
}