using System;

namespace Server.Interface
{
    interface IUser<T> where T : class
    {
        void UpdateName(Guid userId, string newName);
    }
}