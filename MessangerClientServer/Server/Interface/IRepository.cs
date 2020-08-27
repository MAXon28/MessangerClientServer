using System.Collections.Generic;

namespace Server.Interface
{
    interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        void Create(T item);
    }
}