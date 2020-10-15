using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ChatClient.Interface;
using ChatClient.Logic;
using ChatClient.Logic.NotificationLogic;
using ChatClient.Logic.UserLogic;
using ChatClient.View.Game;
using ChatClient.ViewModel.List;
using ChatLibrary;
using ChatLibrary.DataTransferObject;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel.Game
{
    class GameMainPageViewModel : ViewModelBase, IViewModel
    {
        private ServerWorker _serverWorker;

        public GameMainPageViewModel() { }

        public GameMainPageViewModel(string name)
        {
            _serverWorker = ServerWorker.NewInstance();
            Condition = "Visible";
            Name = name;
        }

        public string Condition { get; set; }

        public string Name { get; set; }

        public ICommand PlayWithUser
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var gamePlayViewModel = new GamePlayViewModel(Name, "With user");
                    _serverWorker.RewriteDelegate(gamePlayViewModel);
                    await Task.Run(() => _serverWorker.EventOpenNewPage());
                    Game = new GamePlayView();
                    Game.DataContext = gamePlayViewModel;
                    await Task.Run(_serverWorker.SearchGamer);
                    ViewModel = gamePlayViewModel;
                });
            }
        }

        public ICommand PlayWithComputer
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    var gamePlayViewModel = new GamePlayViewModel(Name, "With computer");
                    _serverWorker.RewriteDelegate(gamePlayViewModel);
                    await Task.Run(() => _serverWorker.EventOpenNewPage());
                    Game = new GamePlayView();
                    Game.DataContext = gamePlayViewModel;
                    ViewModel = gamePlayViewModel;
                });
            }
        }

        public ICommand Rating
        {
            get
            {
                return new RelayCommand( async () =>
                {
                    var gameRatingViewModel = new GameRatingViewModel(Name);
                    _serverWorker.RewriteDelegate(gameRatingViewModel);
                    await Task.Run(() => _serverWorker.EventOpenNewPage());
                    Game = new GameRatingView();
                    Game.DataContext = gameRatingViewModel;
                    await Task.Run(_serverWorker.GetGameRating);
                });
            }
        }

        public FrameworkElement Game { get; set; }

        public GamePlayViewModel ViewModel { get; private set; }

        public void Notification(BinaryReader binaryReader)
        {
            string code = binaryReader.ReadString();
            if (code == "29")
            {
                NotificationTranslator.GetEnteringUserNotification(binaryReader.ReadString(), "Information");
            }
            else if (code == "30")
            {
                NotificationTranslator.PlaySoundNotificationAsync();
                NotificationTranslator.GetNewMessageNotification(binaryReader.ReadString());
            }
            else if (code == "40")
            {
                NotificationTranslator.RewriteDataNotification("Имя пользователя изменено!", "Success");
                Name = binaryReader.ReadString();
            }
            else if (code == "41")
            {
                NotificationTranslator.RewriteDataNotification("Имя пользователя не изменено! Пользователь с таким именем уже зарегистрирован!", "Error");
            }
            else if (code == "42")
            {
                NotificationTranslator.RewriteDataNotification("Логин пользователя изменён!", "Success");
                UserContainer.Login = binaryReader.ReadString();
            }
            else if (code == "43")
            {
                NotificationTranslator.RewriteDataNotification("Логин пользователя не изменён! Пользователь с таким логином уже зарегистрирован!", "Error");
            }
        }
    }
}