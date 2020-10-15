using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ChatClient.Interface;
using ChatClient.Logic;
using ChatClient.Logic.NotificationLogic;
using ChatClient.Logic.UserLogic;
using ChatLibrary.DataTransferObject;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel.Game
{
    class GameRatingViewModel : ViewModelBase, IViewModel
    {
        private ServerWorker _serverWorker;
        private List<RatingDTO> _ratingOverall;
        private List<RatingDTO> _ratingWithUsers;
        private List<RatingDTO> _ratingWithComputer;

        public GameRatingViewModel() {}

        public GameRatingViewModel(string name)
        {
            _serverWorker = ServerWorker.NewInstance();
            Condition = "Visible";
            Name = name;
            IsEnableOverall = false;
            IsEnableUsers = true;
            IsEnableComputer = true;
        }

        public string Condition { get; set; }

        public string Name { get; set; }

        public List<RatingDTO> CurrentRating { get; set; }

        public bool IsEnableOverall { get; set; }

        public bool IsEnableUsers { get; set; }

        public bool IsEnableComputer { get; set; }

        public ICommand Overall
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentRating = _ratingOverall;
                    IsEnableOverall = false;
                    IsEnableUsers = true;
                    IsEnableComputer = true;
                });
            }
        }

        public ICommand WithUsers
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentRating = _ratingWithUsers;
                    IsEnableOverall = true;
                    IsEnableUsers = false;
                    IsEnableComputer = true;
                });
            }
        }

        public ICommand WithComputer
        {
            get
            {
                return new RelayCommand(() =>
                {
                    CurrentRating = _ratingWithComputer;
                    IsEnableOverall = true;
                    IsEnableUsers = true;
                    IsEnableComputer = false;
                });
            }
        }

        public ICommand Cancel
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Condition = "Collapsed";
                });
            }
        }

        public void Notification(BinaryReader binaryReader)
        {
            string code = binaryReader.ReadString();
            if (code == "13-1")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IFormatter formatter = new BinaryFormatter();
                    CurrentRating = (List<RatingDTO>)formatter.Deserialize(_serverWorker.NetworkStream);
                    _ratingOverall = CurrentRating;
                    _ratingWithUsers = (List<RatingDTO>)formatter.Deserialize(_serverWorker.NetworkStream);
                    _ratingWithComputer = (List<RatingDTO>)formatter.Deserialize(_serverWorker.NetworkStream);

                }, DispatcherPriority.Background);
            }
            else if (code == "29")
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