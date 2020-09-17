using System;

namespace Server.Interface
{
    interface IUser<T> where T : class
    {
        void UpdateName(Guid userId, string newName);

        void UpdateGender(Guid userId, string newGender);

        void UpdateLogin(Guid userId, string newLogin);

        void UpdatePassword(Guid userId, string newPassword);

        void UpdatePastOnline(Guid userId, DateTime newPastOnline);
    }
}