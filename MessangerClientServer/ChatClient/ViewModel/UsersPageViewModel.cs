using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChatClient.Interface;
using GalaSoft.MvvmLight;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ChatClient.Logic;
using ChatClient.Logic.NotificationLogic;
using ChatClient.Logic.UserLogic;
using ChatClient.Model;
using ChatClient.ViewModel.List;
using ChatLibrary;
using ChatLibrary.DataTransferObject;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel
{
    class UsersPageViewModel : ViewModelBase, IViewModel
    {
        private ServerWorker _serverWorker;
        private string _textSearch;
        private string _pastAction;

        public UsersPageViewModel() { }

        public UsersPageViewModel(string name)
        {
            _serverWorker = ServerWorker.NewInstance();

            Condition = "Visible";
            Name = name;
            Users = new ObservableCollection<UserViewModel>();
            _pastAction = "";
        }

        public string Condition { get; set; }

        public string Name { get; set; }

        public ObservableCollection<UserViewModel> Users { get; set; }

        public ICommand AllUsers
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (_pastAction != "all users")
                    {
                        TextSearch = "";
                        foreach (var user in Users)
                        {
                            user.Visibility = "Visible";
                        }
                        _pastAction = "all users";
                    }
                });
            }
        }

        public ICommand OnlineUsers
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (_pastAction != "online users")
                    {
                        TextSearch = "";
                        foreach (var user in Users)
                        {
                            if (user.PastDateOnline != "online")
                            {
                                user.Visibility = "Collapsed";
                            }
                        }
                        _pastAction = "online users";
                    }
                });
            }
        }

        public string TextSearch
        {
            get { return _textSearch; }
            set
            {
                if (value != null)
                {
                    _textSearch = value;
                    Search();
                }
                _pastAction = "search";
            }
        }

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
            else if (code == "32")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    IFormatter formatter = new BinaryFormatter();
                    var list = (List<UserDTO>)formatter.Deserialize(_serverWorker.NetworkStream);
                    foreach (var user in list)
                    {
                        string genderForDate = user.Gender == "woman" ? "Была " : "Был ";
                        Users.Add(new UserViewModel
                        {
                            Name = user.Name,
                            CircleColor = user.Gender == "woman" ? (Brush)new BrushConverter().ConvertFromString("#CD5C5C") : (Brush)new BrushConverter().ConvertFromString("#32CD32"),
                            PastDateOnline = user.PastOnline != null ? genderForDate + Day.GetParsedDate(user.PastOnline.ToString()).ToLower() : "online",
                            Visibility = "Visible"
                        });
                    }
                }, DispatcherPriority.Background);
            }
            else if (code == "40")
            {
                NotificationTranslator.RewriteDataNotification("Имя пользователя изменено!", "Success");
                Name = binaryReader.ReadString();
            }
            else if (code == "41")
            {
                NotificationTranslator.RewriteDataNotification("Имя пользователя не изменено! Пользователь с таким именем уже зарегестрирован!", "Error");
            }
            else if (code == "42")
            {
                NotificationTranslator.RewriteDataNotification("Логин пользователя изменён!", "Success");
                UserContainer.Login = binaryReader.ReadString();
            }
            else if (code == "43")
            {
                NotificationTranslator.RewriteDataNotification("Логин пользователя не изменён! Пользователь с таким логином уже зарегестрирован!", "Error");
            }
        }

        private void Search()
        {
            foreach (var user in Users)
            {
                Regex regex = new Regex(TextSearch.ToLower());
                MatchCollection matches = regex.Matches(user.Name.ToLower());
                if (matches.Count == 0)
                {
                    user.Visibility = "Collapsed";
                }
                else
                {
                    user.Visibility = "Visible";
                }
            }
        }
    }
}