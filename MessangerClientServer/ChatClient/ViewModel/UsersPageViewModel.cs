using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChatClient.Interface;
using GalaSoft.MvvmLight;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ChatClient.Logic;
using ChatClient.Logic.NotificationLogic;
using ChatClient.Logic.UserLogic;
using ChatClient.Model;
using ChatClient.ViewModel.List;
using ChatLibrary;
using ChatLibrary.DataTransferObject;

namespace ChatClient.ViewModel
{
    class UsersPageViewModel : ViewModelBase, IViewModel
    {
        private ServerWorker _serverWorker;

        public UsersPageViewModel() { }

        public UsersPageViewModel(string name)
        {
            _serverWorker = ServerWorker.NewInstance();

            Condition = "Visible";
            Name = name;
            Users = new ObservableCollection<UserViewModel>();
        }

        public string Condition { get; set; }

        public string Name { get; set; }

        public ObservableCollection<UserViewModel> Users { get; set; }

        public void Notification(BinaryReader binaryReader)
        {
            string test = binaryReader.ReadString();
            if (test == "29")
            {
                NotificationTranslator.GetEnteringUserNotification(binaryReader.ReadString(), "Information");
            }
            else if (test == "30")
            {
                NotificationTranslator.PlaySoundNotificationAsync();
                NotificationTranslator.GetNewMessageNotification(binaryReader.ReadString());
            }
            else if (test == "32")
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
                            PastDateOnline = user.PastOnline != null ? genderForDate + Day.GetParsedDate(user.PastOnline.ToString()).ToLower() : "online"
                        });
                    }
                }, DispatcherPriority.Background);
            }
            else if (test == "40")
            {
                NotificationTranslator.RewriteDataNotification("Имя пользователя изменено!", "Success");
                Name = binaryReader.ReadString();
            }
            else if (test == "41")
            {
                NotificationTranslator.RewriteDataNotification("Имя пользователя не изменено! Пользователь с таким именем уже зарегестрирован!", "Error");
            }
            else if (test == "42")
            {
                NotificationTranslator.RewriteDataNotification("Логин пользователя изменён!", "Success");
                UserContainer.Login = binaryReader.ReadString();
            }
            else if (test == "43")
            {
                NotificationTranslator.RewriteDataNotification("Логин пользователя не изменён! Пользователь с таким логином уже зарегестрирован!", "Error");
            }
        }
    }
}