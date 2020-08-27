using Server.Repository;

namespace Server.Interface
{
    interface IService
    {
        EFUnitOfWork GetUnitOfWork();

        void SaveAsync();
    }
}