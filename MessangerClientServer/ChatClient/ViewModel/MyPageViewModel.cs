using System.IO;
using ChatClient.Interface;
using GalaSoft.MvvmLight;
using ToastNotifications;
using ToastNotifications.Messages;

namespace ChatClient.ViewModel
{
    class MyPageViewModel : ViewModelBase, IViewModel
    {
        private Notifier _notifier;

        public MyPageViewModel() { }

        public MyPageViewModel(Notifier notifier)
        {
            Condition = "Visible";

            _notifier = notifier;
        }

            public string Condition { get; set; }

        public void Notification(BinaryReader binaryReader)
        {
            if (binaryReader.ReadString() == "67")
            {
                _notifier.ShowInformation($"Новое сообщение!");
            }
        }
    }
}