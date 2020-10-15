using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatClient.Interface;
using ChatClient.Logic;
using ChatClient.Logic.NotificationLogic;
using ChatClient.Logic.UserLogic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace ChatClient.ViewModel
{
    class SettingsPageViewModel : ViewModelBase, IViewModel
    {
        private ServerWorker _serverWorker;
        private List<string> _typesOfNotification;
        private List<int> _typesOfSound;

        public SettingsPageViewModel() { }

        public SettingsPageViewModel(string name)
        {
            _serverWorker = ServerWorker.NewInstance();
            _typesOfNotification = new List<string> {"All on", "Off about entering users", "Off about new messages", "All off"};
            _typesOfSound = new List<int>
            {
                NotificationTranslator.SOUND_OFF,
                NotificationTranslator.IPHONE,
                NotificationTranslator.VKONTAKTE,
                NotificationTranslator.TELEGRAM,
                NotificationTranslator.WHATS_UP_IPHONE,
                NotificationTranslator.WHATS_UP_ANDROID,
                NotificationTranslator.CLASSMATES,
                NotificationTranslator.ASKA
            };
            Condition = "Visible";
            Name = name;
            IndexTypeOfNotification = _typesOfNotification.IndexOf(SettingsContainer.TypeOfNotification);
            IndexTypeOfSound = _typesOfSound.IndexOf(SettingsContainer.TypeOfSoundAtNotificationNewMessage);
        }

        public string Condition { get; set; }

        public string Name { get; set; }

        public int IndexTypeOfNotification { get; set; }

        public int IndexTypeOfSound { get; set; }

        public ICommand Save
        {
            get
            {
                return new RelayCommand( async () =>
                {
                    if (_typesOfNotification[IndexTypeOfNotification] != SettingsContainer.TypeOfNotification)
                    {
                        SettingsContainer.TypeOfNotification = _typesOfNotification[IndexTypeOfNotification];
                        await Task.Run(() => _serverWorker.GetUpdateSettings(SettingsContainer.TypeOfNotification));
                    }

                    if (_typesOfSound[IndexTypeOfSound] != SettingsContainer.TypeOfSoundAtNotificationNewMessage)
                    {
                        SettingsContainer.TypeOfSoundAtNotificationNewMessage = _typesOfSound[IndexTypeOfSound];
                        await Task.Run(() => _serverWorker.GetUpdateSettings(SettingsContainer.TypeOfSoundAtNotificationNewMessage.ToString()));
                    }
                });
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