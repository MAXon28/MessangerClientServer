using System.IO;
using System.Windows.Input;
using ChatClient.Interface;
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
        private Notifier _notifier;

        public MyPageViewModel() { }

        public MyPageViewModel(Notifier notifier, string name)
        {
            Condition = "Visible";

            _notifier = notifier;
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
                    nameDialogView.Show();
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

        public void Notification(BinaryReader binaryReader)
        {
            string test = binaryReader.ReadString();
            if (test == "29")
            {
                _notifier.ShowInformation(binaryReader.ReadString());
            }
            else if (test == "30")
            {
                _notifier.ShowInformation(binaryReader.ReadString());
            }
            else if (test == "40")
            {
                _notifier.ShowInformation("Имя пользователя изменено!");
                Name = binaryReader.ReadString();
            }
            else if (test == "41")
            {
                _notifier.ShowInformation("Имя пользователя не изменено! Пользователь с таким именем уже зарегестрирован!");
            }
        }
    }
}