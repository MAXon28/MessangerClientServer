using System;

namespace Server.Interface
{
    interface ISettings<T> where T : class
    {
        void UpdateTypeOfSoundAtNotificationNewMessage(Guid userId, int typeOfSound);

        void UpdateTypeOfNotification(Guid userId, string typeOfNotification);
    }
}