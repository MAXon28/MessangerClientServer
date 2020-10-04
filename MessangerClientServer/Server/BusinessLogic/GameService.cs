using System;
using System.Threading.Tasks;
using Server.Interface;
using Server.Model;
using Server.Repository;

namespace Server.BusinessLogic
{
    class GameService : IService
    {
        private EFUnitOfWork _efUnitOfWork;

        public GameService()
        {
            _efUnitOfWork = new EFUnitOfWork();
        }

        public GameService(EFUnitOfWork efUnitOfWork)
        {
            _efUnitOfWork = efUnitOfWork;
        }

        public void AddGamer(Guid userId)
        {
            GameStatistic gameStatistic = new GameStatistic
            {
                UserId = userId,
                AllGameWithUsers = 0,
                WinWithUsers = 0,
                AllGameWithComputer = 0,
                WinWithComputer = 0
            };
            _efUnitOfWork.GameRepository.Create(gameStatistic);
        }

        public EFUnitOfWork GetUnitOfWork()
        {
            return _efUnitOfWork;
        }

        public async void SaveAsync()
        {
            await Task.Run(_efUnitOfWork.Save);
        }
    }
}