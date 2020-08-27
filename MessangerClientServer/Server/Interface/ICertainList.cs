using System.Collections.Generic;

namespace Server.Interface
{
    interface ICertainList<T> where T : class
    {
        IEnumerable<T> GetMessagesBeforeId(int id);

        IEnumerable<T> GetMessagesAfterId(int id);
    }
}