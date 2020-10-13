using System.IO;
using System.Media;
using System.Windows.Input;
using ChatClient.Interface;
using ChatClient.Logic.MessageLogic;
using ChatClient.Logic.NotificationLogic;
using ChatClient.Logic.UserLogic;
using ChatClient.View.Dialog;
using ChatClient.ViewModel.Dialog;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ChatClient.ViewModel
{
    class MyPageViewModel : ViewModelBase, IViewModel
    {
        public MyPageViewModel() { }

        public MyPageViewModel(string name)
        {
            Condition = "Visible";

            Name = name;
        }

        public string Condition { get; set; }

        public string Name { get; set; }

        public ICommand RewriteName
        {
            get
            {
                return new RelayCommand(() =>
                {
                    NameDialogView nameDialogView = new NameDialogView(new NameDialogViewModel(Name));
                    nameDialogView.ShowDialog();
                });
            }
        }

        public ICommand RewriteGender
        {
            get
            {
                return new RelayCommand(() =>
                {
                    GenderDialogView genderDialogView = new GenderDialogView(new GenderDialogViewModel());
                    genderDialogView.ShowDialog();
                });
            }
        }

        public ICommand RewriteLogin
        {
            get
            {
                return new RelayCommand(() =>
                {
                    LoginDialogView loginDialogView = new LoginDialogView(new LoginDialogViewModel());
                    loginDialogView.ShowDialog();
                });
            }
        }

        public ICommand RewritePassword
        {
            get
            {
                return new RelayCommand(() =>
                {
                    PasswordDialogView passwordDialogView = new PasswordDialogView(new PasswordDialogViewModel());
                    passwordDialogView.ShowDialog();
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
                //MessagesContainer.RewriteFile(Name);
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
    }
}