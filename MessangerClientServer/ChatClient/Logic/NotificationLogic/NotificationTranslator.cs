using System;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace ChatClient.Logic.NotificationLogic
{
    public static class NotificationTranslator
    {
        private static Notifier _notifier;

        private const int SOUND_OFF = 0;
        private const int IPHONE = 1;
        private const int VKONTAKTE = 2;
        private const int TELEGRAM = 3;
        private const int WHATS_UP_IPHONE = 4;
        private const int WHATS_UP_ANDROID = 5;
        private const int CLASSMATES = 6;
        private const int ASKA = 7;

        private static readonly string ALL_ON = "All on";
        private static readonly string OFF_ABOUT_ENTERING_USERS = "Off about entering users";
        private static readonly string OFF_ABOUT_NEW_MESSAGES = "Off about new messages";
        private static readonly string ALL_OF = "All off";

        public static void CreateNotifier()
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
        }

        public static void GetEnteringUserNotification(string information, string typeNotification)
        {
            string settings = SettingsContainer.TypeOfNotification;
            if (settings == ALL_ON || settings == OFF_ABOUT_NEW_MESSAGES)
            {
                switch (typeNotification)
                {
                    case "Success":
                        _notifier.ShowSuccess(information);
                        break;
                    case "Information":
                        _notifier.ShowInformation(information);
                        break;
                }
            }
        }

        public static void GetNewMessageNotification(string information)
        {
            string settings = SettingsContainer.TypeOfNotification;
            if (settings == ALL_ON || settings == OFF_ABOUT_ENTERING_USERS)
            {
                _notifier.ShowInformation(information);
            }
        }

        public static void RewriteDataNotification(string information, string typeNotification)
        {
            switch (typeNotification)
            {
                case "Success":
                    _notifier.ShowSuccess(information);
                    break;
                case "Error":
                    _notifier.ShowError(information);
                    break;
            }
        }

        public static async void PlaySoundNotificationAsync()
        {
            await Task.Run(() =>
            {
                int settings = SettingsContainer.TypeOfSoundAtNotificationNewMessage;
                SoundPlayer player = new SoundPlayer();
                switch (settings)
                {
                    case SOUND_OFF:
                        break;
                    case IPHONE:
                        player.Stream = Properties.Resources.Iphone;
                        break;
                    case VKONTAKTE:
                        player.Stream = Properties.Resources.Vkontakte;
                        break;
                    case TELEGRAM:
                        player.Stream = Properties.Resources.Telegram;
                        break;
                    case WHATS_UP_IPHONE:
                        player.Stream = Properties.Resources.WhatsUpIphone;
                        break;
                    case WHATS_UP_ANDROID:
                        player.Stream = Properties.Resources.WhatsUpAndroid;
                        break;
                    case CLASSMATES:
                        player.Stream = Properties.Resources.Classmates;
                        break;
                    case ASKA:
                        player.Stream = Properties.Resources.Aska;
                        break;
                }

                player.Play();
            });
        }
    }
}